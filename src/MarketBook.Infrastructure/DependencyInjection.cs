// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
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
/// FA: Ø«Ø¨Øª ÙˆØ§Ø¨Ø³ØªÚ¯ÛŒâ€ŒÙ‡Ø§ÛŒ Ù„Ø§ÛŒÙ‡ Infrastructure Ø±Ø§ ÙØ±Ø§Ù‡Ù… Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
/// </summary>
public static class DependencyInjection
{
/// <summary>
/// EN: Registers Infrastructure services and persistence components.
/// FA: Ø³Ø±ÙˆÛŒØ³â€ŒÙ‡Ø§ Ùˆ Ø§Ø¬Ø²Ø§ÛŒ Ù…Ø§Ù†Ø¯Ú¯Ø§Ø±ÛŒ Ù„Ø§ÛŒÙ‡ Infrastructure Ø±Ø§ Ø«Ø¨Øª Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
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
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IInstrumentRepository, InstrumentRepository>();
        services.AddScoped<IListingRepository, ListingRepository>();
        services.AddScoped<ITradingCalendarRepository, TradingCalendarRepository>();
        services.AddScoped<IMarketPriceRepository, MarketPriceRepository>();
        services.AddScoped<IDailyTradeStatisticsRepository, DailyTradeStatisticsRepository>();
        services.AddScoped<IIntradayPriceTickRepository, IntradayPriceTickRepository>();
        services.AddScoped<IOrderBookSnapshotRepository, OrderBookSnapshotRepository>();

        services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

        return services;
    }
}

