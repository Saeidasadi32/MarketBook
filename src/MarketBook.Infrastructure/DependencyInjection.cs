// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Infrastructure
// Namespace : MarketBook.Infrastructure
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Logging;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Infrastructure.Logging;
using MarketBook.Infrastructure.Persistence.Context;
using MarketBook.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketBook.Infrastructure;

/// <summary>
/// EN: Provides dependency injection registration for Infrastructure services.
/// FA: ثبت وابستگی‌های لایه Infrastructure را فراهم می‌کند.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// EN: Registers Infrastructure services and persistence components.
    /// FA: سرویس‌ها و اجزای ماندگاری لایه Infrastructure را ثبت می‌کند.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext>(
            static provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IExchangeRepository, ExchangeRepository>();
        services.AddScoped<IMarketRepository, MarketRepository>();

        services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

        return services;
    }
}
