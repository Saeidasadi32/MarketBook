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
/// FA: ÃƒËœÃ‚Â«ÃƒËœÃ‚Â¨ÃƒËœÃ‚Âª Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÅ¡Ã‚Â¯Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Å¾ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™Ãƒâ„¢Ã¢â‚¬Â¡ Infrastructure ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ Ãƒâ„¢Ã‚ÂÃƒËœÃ‚Â±ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Â¡Ãƒâ„¢Ã¢â‚¬Â¦ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
/// </summary>
public static class DependencyInjection
{
/// <summary>
/// EN: Registers Infrastructure services and persistence components.
/// FA: ÃƒËœÃ‚Â³ÃƒËœÃ‚Â±Ãƒâ„¢Ã‹â€ Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â³ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§ Ãƒâ„¢Ã‹â€  ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â²ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯ÃƒÅ¡Ã‚Â¯ÃƒËœÃ‚Â§ÃƒËœÃ‚Â±Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Å¾ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™Ãƒâ„¢Ã¢â‚¬Â¡ Infrastructure ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â«ÃƒËœÃ‚Â¨ÃƒËœÃ‚Âª Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
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
        services.AddScoped<IPortfolioTransactionRepository, PortfolioTransactionRepository>();
        services.AddScoped<IPortfolioCashTransactionRepository, PortfolioCashTransactionRepository>();

        services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

        return services;
    }
}



