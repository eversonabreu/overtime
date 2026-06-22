using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Exceptions;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Evertech.Overtime.Application.Services.Implementations;

internal sealed class AuthService(
    IPersonRepository personRepository,
    IPersonTokenRepository personTokenRepository,
    IGroupPersonRepository groupPersonRepository,
    ICryptographyService cryptographyService,
    IDbUnitOfWork unitOfWork,
    IConfiguration configuration) : IAuthService
{
    private const int AccessTokenExpirationMinutes = 15;
    private const int RefreshTokenExpirationDays = 7;

    public async Task<TokenResponseModel> LoginAsync(LoginModel model, CancellationToken cancellationToken = default)
    {
        var person = await FindPersonByCredentialAsync(model.Credential, cancellationToken)
            ?? throw new BusinessException("Credenciais inválidas.");

        if (!person.IsActive)
            throw new BusinessException("Usuário inativo. Entre em contato com o administrador.");

        var plainPassword = cryptographyService.Decrypt(person.Password);
        if (plainPassword != model.Password)
            throw new BusinessException("Credenciais inválidas.");

        return await GenerateTokenPairAsync(person, cancellationToken);
    }

    public async Task<TokenResponseModel> RefreshAsync(RefreshTokenModel model, CancellationToken cancellationToken = default)
    {
        var token = await personTokenRepository.GetActiveByRefreshTokenAsync(model.RefreshToken, cancellationToken)
            ?? throw new BusinessException("Refresh token inválido ou expirado.");

        var person = await personRepository.GetByIdAsync(token.PersonId, cancellationToken)
            ?? throw new BusinessException("Usuário não encontrado.");

        if (!person.IsActive)
            throw new BusinessException("Usuário inativo. Entre em contato com o administrador.");

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var revokedToken = new PersonToken
            {
                Id = token.Id,
                PersonId = token.PersonId,
                RefreshToken = token.RefreshToken,
                ExpiresAt = token.ExpiresAt,
                IsRevoked = true,
                CreatedAt = token.CreatedAt
            };

            await personTokenRepository.UpdateAsync(revokedToken, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }

        return await GenerateTokenPairAsync(person, cancellationToken);
    }

    public async Task RevokeAsync(RevokeTokenModel model, CancellationToken cancellationToken = default)
    {
        var token = await personTokenRepository.GetActiveByRefreshTokenAsync(model.RefreshToken, cancellationToken);
        if (token is null)
            return;

        var revokedToken = new PersonToken
        {
            Id = token.Id,
            PersonId = token.PersonId,
            RefreshToken = token.RefreshToken,
            ExpiresAt = token.ExpiresAt,
            IsRevoked = true,
            CreatedAt = token.CreatedAt
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await personTokenRepository.UpdateAsync(revokedToken, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<Person?> FindPersonByCredentialAsync(string credential, CancellationToken cancellationToken)
    {
        var isEmail = credential.Contains('@');

        return isEmail
            ? await personRepository.FindOneAsync(p => p.Email == credential, cancellationToken)
            : await personRepository.GetByRegistrationAsync(credential, cancellationToken);
    }

    private async Task<TokenResponseModel> GenerateTokenPairAsync(Person person, CancellationToken cancellationToken)
    {
        var leaderGroups = await groupPersonRepository.GetActiveByPersonIdAsync(person.Id, cancellationToken);
        var activeLeaderGroupIds = leaderGroups
            .Where(gp => gp.IsLeader)
            .Select(gp => gp.GroupId.ToString())
            .ToList();

        var isLeader = activeLeaderGroupIds.Count > 0;

        var accessTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(AccessTokenExpirationMinutes);
        var accessToken = GenerateAccessToken(person, isLeader, activeLeaderGroupIds, accessTokenExpiration);
        var refreshToken = Guid.NewGuid().ToString("N");

        var personToken = new PersonToken
        {
            Id = Guid.NewGuid(),
            PersonId = person.Id,
            RefreshToken = refreshToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(RefreshTokenExpirationDays),
            IsRevoked = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await personTokenRepository.AddAsync(personToken, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }

        return new TokenResponseModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiration
        };
    }

    private string GenerateAccessToken(
        Person person,
        bool isLeader,
        IReadOnlyList<string> leaderGroupIds,
        DateTimeOffset expiration)
    {
        var jwtKey = configuration["JwtKey"]
            ?? throw new InvalidOperationException("JwtKey não configurada.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, person.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, person.Email),
            new("name", person.Name),
            new("isAdmin", person.IsAdmin.ToString().ToLower()),
            new("isLeader", isLeader.ToString().ToLower())
        };

        foreach (var groupId in leaderGroupIds)
            claims.Add(new Claim("leaderOfGroups", groupId));

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiration.UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
