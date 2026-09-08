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
/// FA: Client مشترک API و دیتابیس مجزای SQL Server را برای تست‌های Integration فراهم می‌کند.
/// </summary>
public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly MarketBookApiFactory _factory;
/// <summary>
/// EN: Initializes a new integration-test fixture.
/// FA: یک Fixture جدید برای تست‌های Integration ایجاد می‌کند.
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
/// FA: Client متصل به API درون‌حافظه‌ای مارکت‌بوک را دریافت می‌کند.
/// </summary>
public HttpClient Client { get; }
/// <summary>
/// EN: Creates and migrates the isolated integration-test database before the test collection starts.
/// FA: پیش از شروع مجموعه تست، دیتابیس مجزای Integration را ایجاد کرده و Migrationها را اعمال می‌کند.
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
/// FA: همه ارزها را حذف می‌کند تا هر سناریوی Endpoint از وضعیت قطعی و تمیز آغاز شود.
/// </summary>
public async Task ResetCurrenciesAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Removes all instruments so each endpoint scenario starts from a deterministic state.
/// FA: همه ابزارهای مالی را حذف می‌کند تا هر سناریوی Endpoint از وضعیت قطعی و تمیز آغاز شود.
/// </summary>
public async Task ResetInstrumentsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Resets tables used by Listing endpoint scenarios while respecting foreign-key order.
/// FA: جدول‌های مورد استفاده سناریوهای Listing را با رعایت ترتیب کلیدهای خارجی پاک‌سازی می‌کند.
/// </summary>
public async Task ResetListingsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

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
/// FA: وابستگی‌های فعال مورد نیاز تست‌های Endpoint مربوط به Listing را ایجاد می‌کند.
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
/// FA: داده‌های تست تقویم معاملاتی را پیش از Marketهای والد پاک‌سازی می‌کند.
/// </summary>
public async Task ResetTradingCalendarsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<TradingCalendar>().ExecuteDeleteAsync();
        await dbContext.Set<Venue>().ExecuteDeleteAsync();
        await dbContext.Set<Market>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Creates an active market for trading-calendar tests.
/// FA: یک Market فعال برای تست‌های تقویم معاملاتی ایجاد می‌کند.
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
/// <summary>EN: Resets MarketPrice data and Listing dependencies. FA: داده‌های MarketPrice و وابستگی‌های Listing را پاک‌سازی می‌کند.</summary>
public async Task ResetMarketPricesAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        EnsureSafeTestDatabase(dbContext);
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<TradingCalendar>().ExecuteDeleteAsync();
        await dbContext.Set<Venue>().ExecuteDeleteAsync();
        await dbContext.Set<Market>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }
/// <summary>EN: Creates an active Listing for MarketPrice tests. FA: یک Listing فعال برای تست‌های MarketPrice ایجاد می‌کند.</summary>
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
/// EN: Deletes the isolated database and disposes test resources after the collection finishes.
/// FA: پس از پایان مجموعه تست، دیتابیس مجزا را حذف کرده و منابع تست را آزاد می‌کند.
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
/// FA: شناسه وابستگی‌های Seed شده برای سناریوی تست Integration مربوط به Listing را نگه می‌دارد.
/// </summary>
internal sealed record ListingTestSeed(
    string InstrumentId,
    string VenueId,
    string QuoteCurrencyId);
