using System.Data;
using Npgsql;

namespace Evertech.Overtime.Infrastructure.Data;

internal sealed class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new NpgsqlConnection(connectionString);
}