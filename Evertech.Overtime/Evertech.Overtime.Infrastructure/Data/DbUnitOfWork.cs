using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Data;
using Npgsql;
using System.Data;

namespace Evertech.Overtime.Infrastructure.UnitOfWork;

internal sealed class DbUnitOfWork : IDbUnitOfWork
{
    private readonly NpgsqlConnection _connection;
    private NpgsqlTransaction? _transaction;

    public DbUnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connection = (NpgsqlConnection)connectionFactory.CreateConnection();
        _connection.Open();
    }

    public IDbConnection Connection => _connection;

    public IDbTransaction? Transaction => _transaction;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        _transaction = await _connection.BeginTransactionAsync(cancellationToken);

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync();

        await _connection.DisposeAsync();
    }
}