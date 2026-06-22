using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Application.Services.Implementations;
using Evertech.Overtime.Application.Validators;
using Evertech.Overtime.Domain.DI;
using Evertech.Overtime.Infrastructure.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;

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
        services.AddValidatorsFromAssemblyContaining<CreatePersonValidator>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IJourneyService, JourneyService>();
        services.AddScoped<ICompensatoryConversionAppService, CompensatoryConversionAppService>();

        return services;
    }
}