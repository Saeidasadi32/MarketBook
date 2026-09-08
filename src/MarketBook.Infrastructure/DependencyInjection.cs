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
/// FA: Ã˜Â«Ã˜Â¨Ã˜Âª Ã™Ë†Ã˜Â§Ã˜Â¨Ã˜Â³Ã˜ÂªÃšÂ¯Ã›Å’Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Ã™â€žÃ˜Â§Ã›Å’Ã™â€¡ Infrastructure Ã˜Â±Ã˜Â§ Ã™ÂÃ˜Â±Ã˜Â§Ã™â€¡Ã™â€¦ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
public static class DependencyInjection
{
/// <summary>
/// EN: Registers Infrastructure services and persistence components.
/// FA: Ã˜Â³Ã˜Â±Ã™Ë†Ã›Å’Ã˜Â³Ã¢â‚¬Å’Ã™â€¡Ã˜Â§ Ã™Ë† Ã˜Â§Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Ã™â€¦Ã˜Â§Ã™â€ Ã˜Â¯ÃšÂ¯Ã˜Â§Ã˜Â±Ã›Å’ Ã™â€žÃ˜Â§Ã›Å’Ã™â€¡ Infrastructure Ã˜Â±Ã˜Â§ Ã˜Â«Ã˜Â¨Ã˜Âª Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
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
        services.AddScoped<IInvestorRepository, InvestorRepository>();
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();

        services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

        return services;
    }
}

