using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Domain.Exceptions;
using Evertech.Overtime.Domain.Helpers;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using PersonEntity = Evertech.Overtime.Domain.Entities.Person;

namespace Evertech.Overtime.Application.Services.Implementations;

internal sealed class PersonService(
    IPersonRepository personRepository,
    IGroupPersonRepository groupPersonRepository,
    ICryptographyService cryptographyService,
    IDbUnitOfWork unitOfWork,
    IConfiguration configuration) : IPersonService
{
    public async Task<Guid?> CreateFirstAsync(CreateFirstPersonModel model, CancellationToken cancellationToken = default)
    {
        var existing = await personRepository.GetAllAsync(cancellationToken);
        if (existing.Count > 0)
            return null;

        var plainPassword = PasswordGeneratorHelper.GenerateComplex();
        var createdAt = DateTimeOffset.UtcNow;

        var person = new PersonEntity
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Registration = model.Registration,
            Email = model.Email,
            Password = cryptographyService.Encrypt(plainPassword),
            IsActive = true,
            IsPasswordPendingReset = true,
            IsAdmin = true,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };

        return await CreatePersonAndNotifyAsync(person, plainPassword, cancellationToken);
    }

    public async Task<Guid> CreateAsync(CreatePersonModel model, CancellationToken cancellationToken = default)
    {
        var plainPassword = PasswordGeneratorHelper.GenerateComplex();
        var createdAt = DateTimeOffset.UtcNow;

        var person = new PersonEntity
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Registration = model.Registration,
            Email = model.Email,
            Password = cryptographyService.Encrypt(plainPassword),
            IsActive = true,
            IsPasswordPendingReset = true,
            IsAdmin = model.IsAdmin,
            HourlyRate = model.HourlyRate,
            CompensatoryTimeEnabled = model.CompensatoryTimeEnabled,
            MunicipalityId = model.MunicipalityId,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };

        var id = await CreatePersonAndNotifyAsync(person, plainPassword, cancellationToken);
        return id!.Value;
    }

    public async Task<PersonResponseModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(id, cancellationToken);
        return person is null ? null : MapToResponse(person);
    }

    public async Task<OperationResult> UpdateAsync(UpdatePersonModel model, CancellationToken cancellationToken = default)
    {
        var current = await personRepository.GetByIdAsync(model.Id, cancellationToken);
        if (current is null)
            return OperationResult.Failure("A pessoa informada não foi encontrada.");

        if (current.IsActive && !model.IsActive)
        {
            var isLeader = await groupPersonRepository.IsLeaderOfAnyGroupAsync(model.Id, cancellationToken);
            if (isLeader)
                return OperationResult.Failure(
                    "Não é possível desativar esta pessoa pois ela é líder de pelo menos um grupo. Remova sua liderança de todos os grupos antes de desativá-la.");
        }

        var person = new PersonEntity
        {
            Id = current.Id,
            Name = model.Name,
            Registration = model.Registration,
            Email = model.Email,
            Password = current.Password,
            IsActive = model.IsActive,
            IsPasswordPendingReset = current.IsPasswordPendingReset,
            IsAdmin = current.IsAdmin,
            HourlyRate = model.HourlyRate,
            CompensatoryTimeEnabled = model.CompensatoryTimeEnabled,
            MunicipalityId = model.MunicipalityId,
            CreatedAt = current.CreatedAt,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await personRepository.UpdateAsync(person, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task ChangePasswordAsync(ChangePasswordModel model, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(model.PersonId, cancellationToken);
        if (person is null)
            return;

        var currentPlainPassword = cryptographyService.Decrypt(person.Password);
        if (currentPlainPassword != model.CurrentPassword)
            throw new BusinessException("A senha atual informada não confere.");

        await UpdatePasswordAsync(person, model.NewPassword, isPasswordPendingReset: false, sendEmail: false, cancellationToken);
    }

    public async Task RequestPasswordResetAsync(RequestPasswordResetModel model, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.FindOneAsync(p => p.Email == model.Email, cancellationToken);
        if (person is null)
            return;

        var plainPassword = PasswordGeneratorHelper.GenerateComplex();
        await UpdatePasswordAsync(person, plainPassword, isPasswordPendingReset: true, sendEmail: true, cancellationToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordModel model, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(model.PersonId, cancellationToken);
        if (person is null)
            return;

        await UpdatePasswordAsync(person, model.NewPassword, isPasswordPendingReset: false, sendEmail: false, cancellationToken);
    }

    private async Task<Guid?> CreatePersonAndNotifyAsync(
        PersonEntity person,
        string plainPassword,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var id = await personRepository.AddAsync(person, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            var emailSent = BuildWelcomeEmail(person, plainPassword).SendWait();
            if (!emailSent)
            {
                await DeletePersonAfterEmailFailureAsync(id, cancellationToken);
                throw new InvalidOperationException(
                    $"Não foi possível enviar o e-mail de boas-vindas para '{person.Email}'. O cadastro foi desfeito.");
            }

            return id;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task DeletePersonAfterEmailFailureAsync(Guid personId, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await personRepository.DeleteAsync(personId, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task UpdatePasswordAsync(
        PersonEntity person,
        string newPlainPassword,
        bool isPasswordPendingReset,
        bool sendEmail,
        CancellationToken cancellationToken)
    {
        var updated = new PersonEntity
        {
            Id = person.Id,
            Name = person.Name,
            Registration = person.Registration,
            Email = person.Email,
            Password = cryptographyService.Encrypt(newPlainPassword),
            IsActive = person.IsActive,
            IsPasswordPendingReset = isPasswordPendingReset,
            IsAdmin = person.IsAdmin,
            HourlyRate = person.HourlyRate,
            CompensatoryTimeEnabled = person.CompensatoryTimeEnabled,
            MunicipalityId = person.MunicipalityId,
            CreatedAt = person.CreatedAt,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await personRepository.UpdateAsync(updated, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            if (sendEmail)
                BuildPasswordResetEmail(updated, newPlainPassword).Send();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private EmailHelper BuildWelcomeEmail(PersonEntity person, string plainPassword)
    {
        var body = $"""
            <p>Olá {person.Name},</p>
            <p>Bem-vindo(a) ao <strong>Overtime</strong> - Sistema de Controle de Jornada Extra de Trabalho.</p>
            <p>Seus dados de acesso:</p>
            <p>
                E-mail: {person.Email}<br/>
                Senha provisória: <strong>{plainPassword}</strong>
            </p>
            <p>Por segurança, você precisará definir uma nova senha no seu primeiro acesso.</p>
            """;

        return EmailHelper.Create(configuration)
            .WithTitle("Bem-vindo(a) ao Overtime")
            .WithRecipient(person.Email)
            .WithBody(body);
    }

    private EmailHelper BuildPasswordResetEmail(PersonEntity person, string plainPassword)
    {
        var body = $"""
            <p>Olá {person.Name},</p>
            <p>Recebemos uma solicitação de redefinição de senha no <strong>Overtime</strong> - Sistema de Controle de Jornada Extra de Trabalho.</p>
            <p>Sua nova senha provisória:</p>
            <p><strong>{plainPassword}</strong></p>
            <p>Por segurança, você precisará definir uma nova senha no seu próximo acesso.</p>
            <p>Se você não solicitou esta redefinição, entre em contato com o seu administrador ou líder de grupo.</p>
            """;

        return EmailHelper.Create(configuration)
            .WithTitle("Redefinição de senha - Overtime")
            .WithRecipient(person.Email)
            .WithBody(body);
    }

    private static PersonResponseModel MapToResponse(PersonEntity person) => new()
    {
        Id = person.Id,
        Name = person.Name,
        Registration = person.Registration,
        Email = person.Email,
        IsActive = person.IsActive,
        IsPasswordPendingReset = person.IsPasswordPendingReset,
        IsAdmin = person.IsAdmin,
        HourlyRate = person.HourlyRate,
        CompensatoryTimeEnabled = person.CompensatoryTimeEnabled,
        MunicipalityId = person.MunicipalityId,
        CreatedAt = person.CreatedAt,
        UpdatedAt = person.UpdatedAt
    };
}