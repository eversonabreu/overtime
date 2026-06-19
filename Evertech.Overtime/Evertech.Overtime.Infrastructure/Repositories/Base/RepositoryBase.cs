using Dapper;
using Evertech.Overtime.Domain.Entities.Base;
using Evertech.Overtime.Domain.Repositories.Base;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Evertech.Overtime.Infrastructure.Repositories.Base;

[ExcludeFromCodeCoverage]
internal abstract class RepositoryBase<T>(IUnitOfWork unitOfWork) : IRepository<T>
    where T : Entity
{
    protected abstract string TableName { get; }

    protected IUnitOfWork UnitOfWork => unitOfWork;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {TableName} WHERE id = @Id";
        var command = new CommandDefinition(sql, new { Id = id }, unitOfWork.Transaction, cancellationToken: cancellationToken);

        return await unitOfWork.Connection.QuerySingleOrDefaultAsync<T>(command);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {TableName}";
        var command = new CommandDefinition(sql, transaction: unitOfWork.Transaction, cancellationToken: cancellationToken);

        var result = await unitOfWork.Connection.QueryAsync<T>(command);
        return result.AsList();
    }

    public abstract Task<Guid> AddAsync(T entity, CancellationToken cancellationToken = default);

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var current = await GetByIdAsync(entity.Id, cancellationToken);
        if (current is null)
            return;

        var updateCommand = UpdateBuilder.Build(current, entity);
        if (updateCommand is null)
            return;

        var sql = $"UPDATE {TableName} SET {updateCommand.SetClause} WHERE id = @Id";
        var command = new CommandDefinition(sql, updateCommand.Parameters, unitOfWork.Transaction, cancellationToken: cancellationToken);

        await unitOfWork.Connection.ExecuteAsync(command);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sql = $"DELETE FROM {TableName} WHERE id = @Id";
        var command = new CommandDefinition(sql, new { Id = id }, unitOfWork.Transaction, cancellationToken: cancellationToken);

        await unitOfWork.Connection.ExecuteAsync(command);
    }

    public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.AsQueryable().FirstOrDefault(predicate);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.AsQueryable().Where(predicate).ToList();
    }
}