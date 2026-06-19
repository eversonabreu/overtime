using System.Data;

namespace Evertech.Overtime.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}