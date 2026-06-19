using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Domain.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Domain.DI;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<ICryptographyService, CryptographyService>();
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddScoped<IJourneyDecomposerService, JourneyDecomposerService>();
        services.AddScoped<ICompensatoryBalanceService, CompensatoryBalanceService>();
        services.AddScoped<ICompensatoryConversionService, CompensatoryConversionService>();

        return services;
    }
}