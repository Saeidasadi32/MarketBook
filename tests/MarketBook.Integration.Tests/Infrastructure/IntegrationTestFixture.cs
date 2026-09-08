// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Infrastructure
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Calendar.Aggregates;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Instrument.Aggregates;
using MarketBook.Domain.Instrument.Enums;
using MarketBook.Domain.Instrument.ValueObjects;
using MarketBook.Domain.Investor.Aggregates;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.MarketData.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using MarketBook.Domain.Venue.Aggregates;
using MarketBook.Domain.Venue.Enums;
using MarketBook.Domain.Venue.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MarketBook.Integration.Tests.Infrastructure;
/// <summary>
/// EN: Provides the shared API client and isolated SQL Server database for integration tests.
/// FA: Client Ã™â€¦Ã˜Â´Ã˜ÂªÃ˜Â±ÃšÂ© API Ã™Ë† Ã˜Â¯Ã›Å’Ã˜ÂªÃ˜Â§Ã˜Â¨Ã›Å’Ã˜Â³ Ã™â€¦Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ SQL Server Ã˜Â±Ã˜Â§ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜ÂªÃ˜Â³Ã˜ÂªÃ¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Integration Ã™ÂÃ˜Â±Ã˜Â§Ã™â€¡Ã™â€¦ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly MarketBookApiFactory _factory;
/// <summary>
/// EN: Initializes a new integration-test fixture.
/// FA: Ã›Å’ÃšÂ© Fixture Ã˜Â¬Ã˜Â¯Ã›Å’Ã˜Â¯ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜ÂªÃ˜Â³Ã˜ÂªÃ¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Integration Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
public IntegrationTestFixture()
    {
        _factory = new MarketBookApiFactory();

        WebApplicationFactoryClientOptions clientOptions = new()
        {
            BaseAddress = new Uri("https://localhost")
        };

        Client = _factory.CreateClient(clientOptions);
    }
/// <summary>
/// EN: Gets the HTTP client connected to the in-memory MarketBook API.
/// FA: Client Ã™â€¦Ã˜ÂªÃ˜ÂµÃ™â€ž Ã˜Â¨Ã™â€¡ API Ã˜Â¯Ã˜Â±Ã™Ë†Ã™â€ Ã¢â‚¬Å’Ã˜Â­Ã˜Â§Ã™ÂÃ˜Â¸Ã™â€¡Ã¢â‚¬Å’Ã˜Â§Ã›Å’ Ã™â€¦Ã˜Â§Ã˜Â±ÃšÂ©Ã˜ÂªÃ¢â‚¬Å’Ã˜Â¨Ã™Ë†ÃšÂ© Ã˜Â±Ã˜Â§ Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
public HttpClient Client { get; }
/// <summary>
/// EN: Creates and migrates the isolated integration-test database before the test collection starts.
/// FA: Ã™Â¾Ã›Å’Ã˜Â´ Ã˜Â§Ã˜Â² Ã˜Â´Ã˜Â±Ã™Ë†Ã˜Â¹ Ã™â€¦Ã˜Â¬Ã™â€¦Ã™Ë†Ã˜Â¹Ã™â€¡ Ã˜ÂªÃ˜Â³Ã˜ÂªÃ˜Å’ Ã˜Â¯Ã›Å’Ã˜ÂªÃ˜Â§Ã˜Â¨Ã›Å’Ã˜Â³ Ã™â€¦Ã˜Â¬Ã˜Â²Ã˜Â§Ã›Å’ Integration Ã˜Â±Ã˜Â§ Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ ÃšÂ©Ã˜Â±Ã˜Â¯Ã™â€¡ Ã™Ë† MigrationÃ™â€¡Ã˜Â§ Ã˜Â±Ã˜Â§ Ã˜Â§Ã˜Â¹Ã™â€¦Ã˜Â§Ã™â€ž Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
public async Task InitializeAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Database.MigrateAsync();
    }
/// <summary>
/// EN: Removes all currencies so each endpoint scenario can start from a deterministic state.
/// FA: Ã™â€¡Ã™â€¦Ã™â€¡ Ã˜Â§Ã˜Â±Ã˜Â²Ã™â€¡Ã˜Â§ Ã˜Â±Ã˜Â§ Ã˜Â­Ã˜Â°Ã™Â Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â§ Ã™â€¡Ã˜Â± Ã˜Â³Ã™â€ Ã˜Â§Ã˜Â±Ã›Å’Ã™Ë†Ã›Å’ Endpoint Ã˜Â§Ã˜Â² Ã™Ë†Ã˜Â¶Ã˜Â¹Ã›Å’Ã˜Âª Ã™â€šÃ˜Â·Ã˜Â¹Ã›Å’ Ã™Ë† Ã˜ÂªÃ™â€¦Ã›Å’Ã˜Â² Ã˜Â¢Ã˜ÂºÃ˜Â§Ã˜Â² Ã˜Â´Ã™Ë†Ã˜Â¯.
/// </summary>
public async Task ResetCurrenciesAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Removes all instruments so each endpoint scenario starts from a deterministic state.
/// FA: Ã™â€¡Ã™â€¦Ã™â€¡ Ã˜Â§Ã˜Â¨Ã˜Â²Ã˜Â§Ã˜Â±Ã™â€¡Ã˜Â§Ã›Å’ Ã™â€¦Ã˜Â§Ã™â€žÃ›Å’ Ã˜Â±Ã˜Â§ Ã˜Â­Ã˜Â°Ã™Â Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯ Ã˜ÂªÃ˜Â§ Ã™â€¡Ã˜Â± Ã˜Â³Ã™â€ Ã˜Â§Ã˜Â±Ã›Å’Ã™Ë†Ã›Å’ Endpoint Ã˜Â§Ã˜Â² Ã™Ë†Ã˜Â¶Ã˜Â¹Ã›Å’Ã˜Âª Ã™â€šÃ˜Â·Ã˜Â¹Ã›Å’ Ã™Ë† Ã˜ÂªÃ™â€¦Ã›Å’Ã˜Â² Ã˜Â¢Ã˜ÂºÃ˜Â§Ã˜Â² Ã˜Â´Ã™Ë†Ã˜Â¯.
/// </summary>
public async Task ResetInstrumentsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Resets tables used by Listing endpoint scenarios while respecting foreign-key order.
/// FA: Ã˜Â¬Ã˜Â¯Ã™Ë†Ã™â€žÃ¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã˜Â±Ã˜Â¯ Ã˜Â§Ã˜Â³Ã˜ÂªÃ™ÂÃ˜Â§Ã˜Â¯Ã™â€¡ Ã˜Â³Ã™â€ Ã˜Â§Ã˜Â±Ã›Å’Ã™Ë†Ã™â€¡Ã˜Â§Ã›Å’ Listing Ã˜Â±Ã˜Â§ Ã˜Â¨Ã˜Â§ Ã˜Â±Ã˜Â¹Ã˜Â§Ã›Å’Ã˜Âª Ã˜ÂªÃ˜Â±Ã˜ÂªÃ›Å’Ã˜Â¨ ÃšÂ©Ã™â€žÃ›Å’Ã˜Â¯Ã™â€¡Ã˜Â§Ã›Å’ Ã˜Â®Ã˜Â§Ã˜Â±Ã˜Â¬Ã›Å’ Ã™Â¾Ã˜Â§ÃšÂ©Ã¢â‚¬Å’Ã˜Â³Ã˜Â§Ã˜Â²Ã›Å’ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
public async Task ResetListingsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<TradingCalendar>().ExecuteDeleteAsync();
        await dbContext.Set<Venue>().ExecuteDeleteAsync();
        await dbContext.Set<Market>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Creates active persistence dependencies required by Listing endpoint tests.
/// FA: Ã™Ë†Ã˜Â§Ã˜Â¨Ã˜Â³Ã˜ÂªÃšÂ¯Ã›Å’Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Ã™ÂÃ˜Â¹Ã˜Â§Ã™â€ž Ã™â€¦Ã™Ë†Ã˜Â±Ã˜Â¯ Ã™â€ Ã›Å’Ã˜Â§Ã˜Â² Ã˜ÂªÃ˜Â³Ã˜ÂªÃ¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Endpoint Ã™â€¦Ã˜Â±Ã˜Â¨Ã™Ë†Ã˜Â· Ã˜Â¨Ã™â€¡ Listing Ã˜Â±Ã˜Â§ Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
internal async Task<ListingTestSeed> CreateListingSeedAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        string suffix =
            Guid.NewGuid()
                .ToString("N")[..8]
                .ToUpperInvariant();

        Market market = Market.Create(
            new MarketCode($"M{suffix}"),
            $"Listing Test Market {suffix}");

        Venue venue = Venue.Create(
            market.Id,
            new VenueCode($"V{suffix}"),
            $"Listing Test Venue {suffix}",
            VenueType.Exchange);

        Currency currency = Currency.Create(
            new CurrencyCode("USD"),
            "US Dollar",
            2);

        Instrument instrument = Instrument.Create(
            new InstrumentName($"Listing Test Instrument {suffix}"),
            new AssetClass("Equity"),
            InstrumentType.Stock,
            new InstrumentCategory("Equity"));

        await dbContext.Set<Market>().AddAsync(market);
        await dbContext.Set<Venue>().AddAsync(venue);
        await dbContext.Set<Currency>().AddAsync(currency);
        await dbContext.Set<Instrument>().AddAsync(instrument);

        await dbContext.SaveChangesAsync();

        return new ListingTestSeed(
            instrument.Id.Value.ToString(),
            venue.Id.Value.ToString(),
            currency.Id.Value.ToString());
    }
/// <summary>
/// EN: Resets trading-calendar test data before parent markets.
/// FA: Ã˜Â¯Ã˜Â§Ã˜Â¯Ã™â€¡Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Ã˜ÂªÃ˜Â³Ã˜Âª Ã˜ÂªÃ™â€šÃ™Ë†Ã›Å’Ã™â€¦ Ã™â€¦Ã˜Â¹Ã˜Â§Ã™â€¦Ã™â€žÃ˜Â§Ã˜ÂªÃ›Å’ Ã˜Â±Ã˜Â§ Ã™Â¾Ã›Å’Ã˜Â´ Ã˜Â§Ã˜Â² MarketÃ™â€¡Ã˜Â§Ã›Å’ Ã™Ë†Ã˜Â§Ã™â€žÃ˜Â¯ Ã™Â¾Ã˜Â§ÃšÂ©Ã¢â‚¬Å’Ã˜Â³Ã˜Â§Ã˜Â²Ã›Å’ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
public async Task ResetTradingCalendarsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<TradingCalendar>().ExecuteDeleteAsync();
        await dbContext.Set<Venue>().ExecuteDeleteAsync();
        await dbContext.Set<Market>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Creates an active market for trading-calendar tests.
/// FA: Ã›Å’ÃšÂ© Market Ã™ÂÃ˜Â¹Ã˜Â§Ã™â€ž Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜ÂªÃ˜Â³Ã˜ÂªÃ¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Ã˜ÂªÃ™â€šÃ™Ë†Ã›Å’Ã™â€¦ Ã™â€¦Ã˜Â¹Ã˜Â§Ã™â€¦Ã™â€žÃ˜Â§Ã˜ÂªÃ›Å’ Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
internal async Task<string> CreateTradingCalendarMarketAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        string suffix =
            Guid.NewGuid()
                .ToString("N")[..8]
                .ToUpperInvariant();

        Market market = Market.Create(
            new MarketCode($"C{suffix}"),
            $"Calendar Test Market {suffix}");

        await dbContext.Set<Market>().AddAsync(market);
        await dbContext.SaveChangesAsync();

        return market.Id.Value.ToString();
    }
/// <summary>EN: Resets MarketPrice data and Listing dependencies. FA: Ã˜Â¯Ã˜Â§Ã˜Â¯Ã™â€¡Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ MarketPrice Ã™Ë† Ã™Ë†Ã˜Â§Ã˜Â¨Ã˜Â³Ã˜ÂªÃšÂ¯Ã›Å’Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Listing Ã˜Â±Ã˜Â§ Ã™Â¾Ã˜Â§ÃšÂ©Ã¢â‚¬Å’Ã˜Â³Ã˜Â§Ã˜Â²Ã›Å’ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.</summary>
public async Task ResetMarketPricesAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        EnsureSafeTestDatabase(dbContext);
        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<TradingCalendar>().ExecuteDeleteAsync();
        await dbContext.Set<Venue>().ExecuteDeleteAsync();
        await dbContext.Set<Market>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }
/// <summary>EN: Creates an active Listing for MarketPrice tests. FA: Ã›Å’ÃšÂ© Listing Ã™ÂÃ˜Â¹Ã˜Â§Ã™â€ž Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜ÂªÃ˜Â³Ã˜ÂªÃ¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ MarketPrice Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.</summary>
internal async Task<string> CreateMarketPriceListingAsync()
    {
        ListingTestSeed seed = await CreateListingSeedAsync();
        using IServiceScope scope = _factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        EnsureSafeTestDatabase(dbContext);
        Listing listing = Listing.Create(InstrumentId.Parse(seed.InstrumentId), VenueId.Parse(seed.VenueId), CurrencyId.Parse(seed.QuoteCurrencyId), new MarketBook.Domain.Listing.ValueObjects.TradingSymbol("MPTEST"), 1m, 0);
        await dbContext.Set<Listing>().AddAsync(listing);
        await dbContext.SaveChangesAsync();
        return listing.Id.Value.ToString();
    }
/// <summary>
/// EN: Resets daily trade-statistics data and Listing dependencies.
/// FA: Ã˜Â¯Ã˜Â§Ã˜Â¯Ã™â€¡Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Ã˜Â¢Ã™â€¦Ã˜Â§Ã˜Â± Ã™â€¦Ã˜Â¹Ã˜Â§Ã™â€¦Ã™â€žÃ˜Â§Ã˜Âª Ã˜Â±Ã™Ë†Ã˜Â²Ã˜Â§Ã™â€ Ã™â€¡ Ã™Ë† Ã™Ë†Ã˜Â§Ã˜Â¨Ã˜Â³Ã˜ÂªÃšÂ¯Ã›Å’Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Listing Ã˜Â±Ã˜Â§ Ã™Â¾Ã˜Â§ÃšÂ©Ã¢â‚¬Å’Ã˜Â³Ã˜Â§Ã˜Â²Ã›Å’ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
public async Task ResetDailyTradeStatisticsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<TradingCalendar>().ExecuteDeleteAsync();
        await dbContext.Set<Venue>().ExecuteDeleteAsync();
        await dbContext.Set<Market>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }

/// <summary>
/// EN: Creates an active Listing for daily trade-statistics tests.
/// FA: Ã›Å’ÃšÂ© Listing Ã™ÂÃ˜Â¹Ã˜Â§Ã™â€ž Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜ÂªÃ˜Â³Ã˜ÂªÃ¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Ã˜Â¢Ã™â€¦Ã˜Â§Ã˜Â± Ã™â€¦Ã˜Â¹Ã˜Â§Ã™â€¦Ã™â€žÃ˜Â§Ã˜Âª Ã˜Â±Ã™Ë†Ã˜Â²Ã˜Â§Ã™â€ Ã™â€¡ Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
internal async Task<string> CreateDailyTradeStatisticsListingAsync()
    {
        ListingTestSeed seed = await CreateListingSeedAsync();

        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        Listing listing = Listing.Create(
            InstrumentId.Parse(seed.InstrumentId),
            VenueId.Parse(seed.VenueId),
            CurrencyId.Parse(seed.QuoteCurrencyId),
            new MarketBook.Domain.Listing.ValueObjects.TradingSymbol("DTSTAT"),
            1m,
            0);

        await dbContext.Set<Listing>().AddAsync(listing);
        await dbContext.SaveChangesAsync();

        return listing.Id.Value.ToString();
    }

/// <summary>
/// EN: Resets intraday price ticks and their Listing dependencies.
/// FA: TickÃ™â€¡Ã˜Â§Ã›Å’ Ã™â€šÃ›Å’Ã™â€¦Ã˜Âª Ã˜Â¯Ã˜Â±Ã™Ë†Ã™â€ Ã¢â‚¬Å’Ã˜Â±Ã™Ë†Ã˜Â²Ã›Å’ Ã™Ë† Ã™Ë†Ã˜Â§Ã˜Â¨Ã˜Â³Ã˜ÂªÃšÂ¯Ã›Å’Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Listing Ã˜Â±Ã˜Â§ Ã™Â¾Ã˜Â§ÃšÂ©Ã¢â‚¬Å’Ã˜Â³Ã˜Â§Ã˜Â²Ã›Å’ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
public async Task ResetIntradayPriceTicksAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<TradingCalendar>().ExecuteDeleteAsync();
        await dbContext.Set<Venue>().ExecuteDeleteAsync();
        await dbContext.Set<Market>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }

/// <summary>
/// EN: Creates an active Listing for intraday price-tick tests.
/// FA: Ã›Å’ÃšÂ© Listing Ã™ÂÃ˜Â¹Ã˜Â§Ã™â€ž Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜ÂªÃ˜Â³Ã˜ÂªÃ¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Tick Ã™â€šÃ›Å’Ã™â€¦Ã˜Âª Ã˜Â¯Ã˜Â±Ã™Ë†Ã™â€ Ã¢â‚¬Å’Ã˜Â±Ã™Ë†Ã˜Â²Ã›Å’ Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
internal async Task<string> CreateIntradayPriceTickListingAsync()
    {
        ListingTestSeed seed = await CreateListingSeedAsync();

        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        Listing listing = Listing.Create(
            InstrumentId.Parse(seed.InstrumentId),
            VenueId.Parse(seed.VenueId),
            CurrencyId.Parse(seed.QuoteCurrencyId),
            new MarketBook.Domain.Listing.ValueObjects.TradingSymbol("IPTICK"),
            1m,
            0);

        await dbContext.Set<Listing>().AddAsync(listing);
        await dbContext.SaveChangesAsync();

        return listing.Id.Value.ToString();
    }
/// <summary>
/// EN: Deletes the isolated database and disposes test resources after the collection finishes.
/// FA: Ã™Â¾Ã˜Â³ Ã˜Â§Ã˜Â² Ã™Â¾Ã˜Â§Ã›Å’Ã˜Â§Ã™â€  Ã™â€¦Ã˜Â¬Ã™â€¦Ã™Ë†Ã˜Â¹Ã™â€¡ Ã˜ÂªÃ˜Â³Ã˜ÂªÃ˜Å’ Ã˜Â¯Ã›Å’Ã˜ÂªÃ˜Â§Ã˜Â¨Ã›Å’Ã˜Â³ Ã™â€¦Ã˜Â¬Ã˜Â²Ã˜Â§ Ã˜Â±Ã˜Â§ Ã˜Â­Ã˜Â°Ã™Â ÃšÂ©Ã˜Â±Ã˜Â¯Ã™â€¡ Ã™Ë† Ã™â€¦Ã™â€ Ã˜Â§Ã˜Â¨Ã˜Â¹ Ã˜ÂªÃ˜Â³Ã˜Âª Ã˜Â±Ã˜Â§ Ã˜Â¢Ã˜Â²Ã˜Â§Ã˜Â¯ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.
/// </summary>
/// <summary>EN: Resets OrderBookSnapshot data and Listing dependencies. FA: Ã˜Â¯Ã˜Â§Ã˜Â¯Ã™â€¡Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ OrderBookSnapshot Ã™Ë† Ã™Ë†Ã˜Â§Ã˜Â¨Ã˜Â³Ã˜ÂªÃšÂ¯Ã›Å’Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Listing Ã˜Â±Ã˜Â§ Ã™Â¾Ã˜Â§ÃšÂ©Ã¢â‚¬Å’Ã˜Â³Ã˜Â§Ã˜Â²Ã›Å’ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.</summary>
public async Task ResetOrderBookSnapshotsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        EnsureSafeTestDatabase(dbContext);
        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<TradingCalendar>().ExecuteDeleteAsync();
        await dbContext.Set<Venue>().ExecuteDeleteAsync();
        await dbContext.Set<Market>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }

/// <summary>EN: Creates an active Listing for OrderBookSnapshot tests. FA: Ã›Å’ÃšÂ© Listing Ã™ÂÃ˜Â¹Ã˜Â§Ã™â€ž Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜ÂªÃ˜Â³Ã˜ÂªÃ¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ OrderBookSnapshot Ã˜Â§Ã›Å’Ã˜Â¬Ã˜Â§Ã˜Â¯ Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯.</summary>
internal async Task<string> CreateOrderBookSnapshotListingAsync()
    {
        ListingTestSeed seed = await CreateListingSeedAsync();
        using IServiceScope scope = _factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        EnsureSafeTestDatabase(dbContext);
        Listing listing = Listing.Create(
            InstrumentId.Parse(seed.InstrumentId),
            VenueId.Parse(seed.VenueId),
            CurrencyId.Parse(seed.QuoteCurrencyId),
            new MarketBook.Domain.Listing.ValueObjects.TradingSymbol("OBSNAP"),
            1m,
            2);
        await dbContext.Set<Listing>().AddAsync(listing);
        await dbContext.SaveChangesAsync();
        return listing.Id.Value.ToString();
    }
/// <summary>
/// EN: Removes Investor rows from the isolated integration-test database.
/// FA: Removes Investor rows from the isolated integration-test database.
/// </summary>
public async Task ResetInvestorsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<Portfolio>().ExecuteDeleteAsync();
        await dbContext.Set<Investor>().ExecuteDeleteAsync();
    }
public async Task DisposeAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Database.EnsureDeletedAsync();

        Client.Dispose();
        _factory.Dispose();
    }

    private static void EnsureSafeTestDatabase(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        string databaseName = dbContext.Database.GetDbConnection().Database;

        if (!MarketBookApiFactory.IsIntegrationTestDatabase(databaseName))
        {
            throw new InvalidOperationException(
                $"Integration tests refused to mutate database '{databaseName}'. " +
                $"Expected a database name beginning with '{MarketBookApiFactory.TestDatabasePrefix}'.");
        }
    }
}
/// <summary>
/// EN: Carries identifiers of dependencies seeded for a Listing integration-test scenario.
/// FA: Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™Ë†Ã˜Â§Ã˜Â¨Ã˜Â³Ã˜ÂªÃšÂ¯Ã›Å’Ã¢â‚¬Å’Ã™â€¡Ã˜Â§Ã›Å’ Seed Ã˜Â´Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â³Ã™â€ Ã˜Â§Ã˜Â±Ã›Å’Ã™Ë†Ã›Å’ Ã˜ÂªÃ˜Â³Ã˜Âª Integration Ã™â€¦Ã˜Â±Ã˜Â¨Ã™Ë†Ã˜Â· Ã˜Â¨Ã™â€¡ Listing Ã˜Â±Ã˜Â§ Ã™â€ ÃšÂ¯Ã™â€¡ Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜Â¯Ã˜Â§Ã˜Â±Ã˜Â¯.
/// </summary>
internal sealed record ListingTestSeed(
    string InstrumentId,
    string VenueId,
    string QuoteCurrencyId);

