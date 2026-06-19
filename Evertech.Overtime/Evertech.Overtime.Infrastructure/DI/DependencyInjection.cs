using Dapper;
using Evertech.Overtime.Domain.Interfaces;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Data;
using Evertech.Overtime.Infrastructure.Data.TypeHandlers;
using Evertech.Overtime.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Infrastructure.DI;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterDapperTypeHandlers();

        var connectionString = configuration.GetConnectionString("DbOvertime")
            ?? throw new InvalidOperationException("Connection string 'DbOvertime' is not configured.");

        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));

        services.AddScoped<IDbUnitOfWork, UnitOfWork.DbUnitOfWork>();

        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IJourneyRepository, JourneyRepository>();
        services.AddScoped<INationalHolidayRepository, NationalHolidayRepository>();
        services.AddScoped<IStateHolidayRepository, StateHolidayRepository>();
        services.AddScoped<IMunicipalityHolidayRepository, MunicipalityHolidayRepository>();
        services.AddScoped<ICompensatoryConversionRepository, CompensatoryConversionRepository>();

        return services;
    }

    private static void RegisterDapperTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new EntryTypeHandler());
        SqlMapper.AddTypeHandler(new ConversionTypeHandler());
    }
}