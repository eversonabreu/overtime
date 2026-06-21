using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.DI;
using Evertech.Overtime.Infrastructure.DI;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Application.Services.Implementations;

namespace Evertech.Overtime.Application.Configurations;

[ExcludeFromCodeCoverage]
public static class RegisterServices
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration)
                .AddDomain()
                .AddApplicationServices();

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IGroupService, GroupService>();

        return services;
    }
}