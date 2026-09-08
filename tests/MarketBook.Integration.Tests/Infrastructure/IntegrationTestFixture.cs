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
/// FA: Client Ù…Ø´ØªØ±Ú© API Ùˆ Ø¯ÛŒØªØ§Ø¨ÛŒØ³ Ù…Ø¬Ø²Ø§ÛŒ SQL Server Ø±Ø§ Ø¨Ø±Ø§ÛŒ ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Integration ÙØ±Ø§Ù‡Ù… Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
/// </summary>
public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly MarketBookApiFactory _factory;
/// <summary>
/// EN: Initializes a new integration-test fixture.
/// FA: ÛŒÚ© Fixture Ø¬Ø¯ÛŒØ¯ Ø¨Ø±Ø§ÛŒ ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Integration Ø§ÛŒØ¬Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
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
/// FA: Client Ù…ØªØµÙ„ Ø¨Ù‡ API Ø¯Ø±ÙˆÙ†â€ŒØ­Ø§ÙØ¸Ù‡â€ŒØ§ÛŒ Ù…Ø§Ø±Ú©Øªâ€ŒØ¨ÙˆÚ© Ø±Ø§ Ø¯Ø±ÛŒØ§ÙØª Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
/// </summary>
public HttpClient Client { get; }
/// <summary>
/// EN: Creates and migrates the isolated integration-test database before the test collection starts.
/// FA: Ù¾ÛŒØ´ Ø§Ø² Ø´Ø±ÙˆØ¹ Ù…Ø¬Ù…ÙˆØ¹Ù‡ ØªØ³ØªØŒ Ø¯ÛŒØªØ§Ø¨ÛŒØ³ Ù…Ø¬Ø²Ø§ÛŒ Integration Ø±Ø§ Ø§ÛŒØ¬Ø§Ø¯ Ú©Ø±Ø¯Ù‡ Ùˆ MigrationÙ‡Ø§ Ø±Ø§ Ø§Ø¹Ù…Ø§Ù„ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
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
/// FA: Ù‡Ù…Ù‡ Ø§Ø±Ø²Ù‡Ø§ Ø±Ø§ Ø­Ø°Ù Ù…ÛŒâ€ŒÚ©Ù†Ø¯ ØªØ§ Ù‡Ø± Ø³Ù†Ø§Ø±ÛŒÙˆÛŒ Endpoint Ø§Ø² ÙˆØ¶Ø¹ÛŒØª Ù‚Ø·Ø¹ÛŒ Ùˆ ØªÙ…ÛŒØ² Ø¢ØºØ§Ø² Ø´ÙˆØ¯.
/// </summary>
public async Task ResetCurrenciesAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Removes all instruments so each endpoint scenario starts from a deterministic state.
/// FA: Ù‡Ù…Ù‡ Ø§Ø¨Ø²Ø§Ø±Ù‡Ø§ÛŒ Ù…Ø§Ù„ÛŒ Ø±Ø§ Ø­Ø°Ù Ù…ÛŒâ€ŒÚ©Ù†Ø¯ ØªØ§ Ù‡Ø± Ø³Ù†Ø§Ø±ÛŒÙˆÛŒ Endpoint Ø§Ø² ÙˆØ¶Ø¹ÛŒØª Ù‚Ø·Ø¹ÛŒ Ùˆ ØªÙ…ÛŒØ² Ø¢ØºØ§Ø² Ø´ÙˆØ¯.
/// </summary>
public async Task ResetInstrumentsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Resets tables used by Listing endpoint scenarios while respecting foreign-key order.
/// FA: Ø¬Ø¯ÙˆÙ„â€ŒÙ‡Ø§ÛŒ Ù…ÙˆØ±Ø¯ Ø§Ø³ØªÙØ§Ø¯Ù‡ Ø³Ù†Ø§Ø±ÛŒÙˆÙ‡Ø§ÛŒ Listing Ø±Ø§ Ø¨Ø§ Ø±Ø¹Ø§ÛŒØª ØªØ±ØªÛŒØ¨ Ú©Ù„ÛŒØ¯Ù‡Ø§ÛŒ Ø®Ø§Ø±Ø¬ÛŒ Ù¾Ø§Ú©â€ŒØ³Ø§Ø²ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
/// </summary>
public async Task ResetListingsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

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
/// FA: ÙˆØ§Ø¨Ø³ØªÚ¯ÛŒâ€ŒÙ‡Ø§ÛŒ ÙØ¹Ø§Ù„ Ù…ÙˆØ±Ø¯ Ù†ÛŒØ§Ø² ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Endpoint Ù…Ø±Ø¨ÙˆØ· Ø¨Ù‡ Listing Ø±Ø§ Ø§ÛŒØ¬Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
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
/// FA: Ø¯Ø§Ø¯Ù‡â€ŒÙ‡Ø§ÛŒ ØªØ³Øª ØªÙ‚ÙˆÛŒÙ… Ù…Ø¹Ø§Ù…Ù„Ø§ØªÛŒ Ø±Ø§ Ù¾ÛŒØ´ Ø§Ø² MarketÙ‡Ø§ÛŒ ÙˆØ§Ù„Ø¯ Ù¾Ø§Ú©â€ŒØ³Ø§Ø²ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
/// </summary>
public async Task ResetTradingCalendarsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

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
/// FA: ÛŒÚ© Market ÙØ¹Ø§Ù„ Ø¨Ø±Ø§ÛŒ ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ ØªÙ‚ÙˆÛŒÙ… Ù…Ø¹Ø§Ù…Ù„Ø§ØªÛŒ Ø§ÛŒØ¬Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
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
/// <summary>EN: Resets MarketPrice data and Listing dependencies. FA: Ø¯Ø§Ø¯Ù‡â€ŒÙ‡Ø§ÛŒ MarketPrice Ùˆ ÙˆØ§Ø¨Ø³ØªÚ¯ÛŒâ€ŒÙ‡Ø§ÛŒ Listing Ø±Ø§ Ù¾Ø§Ú©â€ŒØ³Ø§Ø²ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.</summary>
public async Task ResetMarketPricesAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        EnsureSafeTestDatabase(dbContext);
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
/// <summary>EN: Creates an active Listing for MarketPrice tests. FA: ÛŒÚ© Listing ÙØ¹Ø§Ù„ Ø¨Ø±Ø§ÛŒ ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ MarketPrice Ø§ÛŒØ¬Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.</summary>
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
/// FA: Ø¯Ø§Ø¯Ù‡â€ŒÙ‡Ø§ÛŒ Ø¢Ù…Ø§Ø± Ù…Ø¹Ø§Ù…Ù„Ø§Øª Ø±ÙˆØ²Ø§Ù†Ù‡ Ùˆ ÙˆØ§Ø¨Ø³ØªÚ¯ÛŒâ€ŒÙ‡Ø§ÛŒ Listing Ø±Ø§ Ù¾Ø§Ú©â€ŒØ³Ø§Ø²ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
/// </summary>
public async Task ResetDailyTradeStatisticsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

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
/// FA: ÛŒÚ© Listing ÙØ¹Ø§Ù„ Ø¨Ø±Ø§ÛŒ ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Ø¢Ù…Ø§Ø± Ù…Ø¹Ø§Ù…Ù„Ø§Øª Ø±ÙˆØ²Ø§Ù†Ù‡ Ø§ÛŒØ¬Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
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
/// FA: TickÙ‡Ø§ÛŒ Ù‚ÛŒÙ…Øª Ø¯Ø±ÙˆÙ†â€ŒØ±ÙˆØ²ÛŒ Ùˆ ÙˆØ§Ø¨Ø³ØªÚ¯ÛŒâ€ŒÙ‡Ø§ÛŒ Listing Ø±Ø§ Ù¾Ø§Ú©â€ŒØ³Ø§Ø²ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
/// </summary>
public async Task ResetIntradayPriceTicksAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

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
/// FA: ÛŒÚ© Listing ÙØ¹Ø§Ù„ Ø¨Ø±Ø§ÛŒ ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Tick Ù‚ÛŒÙ…Øª Ø¯Ø±ÙˆÙ†â€ŒØ±ÙˆØ²ÛŒ Ø§ÛŒØ¬Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
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
/// FA: Ù¾Ø³ Ø§Ø² Ù¾Ø§ÛŒØ§Ù† Ù…Ø¬Ù…ÙˆØ¹Ù‡ ØªØ³ØªØŒ Ø¯ÛŒØªØ§Ø¨ÛŒØ³ Ù…Ø¬Ø²Ø§ Ø±Ø§ Ø­Ø°Ù Ú©Ø±Ø¯Ù‡ Ùˆ Ù…Ù†Ø§Ø¨Ø¹ ØªØ³Øª Ø±Ø§ Ø¢Ø²Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
/// </summary>
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
/// FA: Ø´Ù†Ø§Ø³Ù‡ ÙˆØ§Ø¨Ø³ØªÚ¯ÛŒâ€ŒÙ‡Ø§ÛŒ Seed Ø´Ø¯Ù‡ Ø¨Ø±Ø§ÛŒ Ø³Ù†Ø§Ø±ÛŒÙˆÛŒ ØªØ³Øª Integration Ù…Ø±Ø¨ÙˆØ· Ø¨Ù‡ Listing Ø±Ø§ Ù†Ú¯Ù‡ Ù…ÛŒâ€ŒØ¯Ø§Ø±Ø¯.
/// </summary>
internal sealed record ListingTestSeed(
    string InstrumentId,
    string VenueId,
    string QuoteCurrencyId);

