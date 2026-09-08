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
/// FA: Client Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â´ÃƒËœÃ‚ÂªÃƒËœÃ‚Â±ÃƒÅ¡Ã‚Â© API Ãƒâ„¢Ã‹â€  ÃƒËœÃ‚Â¯Ãƒâ€ºÃ…â€™ÃƒËœÃ‚ÂªÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â³ Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â²ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ SQL Server ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Integration Ãƒâ„¢Ã‚ÂÃƒËœÃ‚Â±ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Â¡Ãƒâ„¢Ã¢â‚¬Â¦ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
/// </summary>
public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly MarketBookApiFactory _factory;
/// <summary>
/// EN: Initializes a new integration-test fixture.
/// FA: Ãƒâ€ºÃ…â€™ÃƒÅ¡Ã‚Â© Fixture ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â¯Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â¯ ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Integration ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
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
/// FA: Client Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚ÂªÃƒËœÃ‚ÂµÃƒâ„¢Ã¢â‚¬Å¾ ÃƒËœÃ‚Â¨Ãƒâ„¢Ã¢â‚¬Â¡ API ÃƒËœÃ‚Â¯ÃƒËœÃ‚Â±Ãƒâ„¢Ã‹â€ Ãƒâ„¢Ã¢â‚¬Â ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â­ÃƒËœÃ‚Â§Ãƒâ„¢Ã‚ÂÃƒËœÃ‚Â¸Ãƒâ„¢Ã¢â‚¬Â¡ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â§ÃƒËœÃ‚Â±ÃƒÅ¡Ã‚Â©ÃƒËœÃ‚ÂªÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â¨Ãƒâ„¢Ã‹â€ ÃƒÅ¡Ã‚Â© ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â¯ÃƒËœÃ‚Â±Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â§Ãƒâ„¢Ã‚ÂÃƒËœÃ‚Âª Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
/// </summary>
public HttpClient Client { get; }
/// <summary>
/// EN: Creates and migrates the isolated integration-test database before the test collection starts.
/// FA: Ãƒâ„¢Ã‚Â¾Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â´ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â² ÃƒËœÃ‚Â´ÃƒËœÃ‚Â±Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â¹ Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â¬Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â¹Ãƒâ„¢Ã¢â‚¬Â¡ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒËœÃ…â€™ ÃƒËœÃ‚Â¯Ãƒâ€ºÃ…â€™ÃƒËœÃ‚ÂªÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â³ Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â²ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Integration ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯ ÃƒÅ¡Ã‚Â©ÃƒËœÃ‚Â±ÃƒËœÃ‚Â¯Ãƒâ„¢Ã¢â‚¬Â¡ Ãƒâ„¢Ã‹â€  MigrationÃƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¹Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Å¾ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
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
/// FA: Ãƒâ„¢Ã¢â‚¬Â¡Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã¢â‚¬Â¡ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â±ÃƒËœÃ‚Â²Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â­ÃƒËœÃ‚Â°Ãƒâ„¢Ã‚Â Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â§ Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â± ÃƒËœÃ‚Â³Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â§ÃƒËœÃ‚Â±Ãƒâ€ºÃ…â€™Ãƒâ„¢Ã‹â€ Ãƒâ€ºÃ…â€™ Endpoint ÃƒËœÃ‚Â§ÃƒËœÃ‚Â² Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â¶ÃƒËœÃ‚Â¹Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Âª Ãƒâ„¢Ã¢â‚¬Å¡ÃƒËœÃ‚Â·ÃƒËœÃ‚Â¹Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã‹â€  ÃƒËœÃ‚ÂªÃƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â² ÃƒËœÃ‚Â¢ÃƒËœÃ‚ÂºÃƒËœÃ‚Â§ÃƒËœÃ‚Â² ÃƒËœÃ‚Â´Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â¯.
/// </summary>
public async Task ResetCurrenciesAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<PortfolioTransaction>().ExecuteDeleteAsync();
        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Removes all instruments so each endpoint scenario starts from a deterministic state.
/// FA: Ãƒâ„¢Ã¢â‚¬Â¡Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã¢â‚¬Â¡ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â²ÃƒËœÃ‚Â§ÃƒËœÃ‚Â±Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Å¾Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â­ÃƒËœÃ‚Â°Ãƒâ„¢Ã‚Â Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â§ Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â± ÃƒËœÃ‚Â³Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â§ÃƒËœÃ‚Â±Ãƒâ€ºÃ…â€™Ãƒâ„¢Ã‹â€ Ãƒâ€ºÃ…â€™ Endpoint ÃƒËœÃ‚Â§ÃƒËœÃ‚Â² Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â¶ÃƒËœÃ‚Â¹Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Âª Ãƒâ„¢Ã¢â‚¬Å¡ÃƒËœÃ‚Â·ÃƒËœÃ‚Â¹Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã‹â€  ÃƒËœÃ‚ÂªÃƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â² ÃƒËœÃ‚Â¢ÃƒËœÃ‚ÂºÃƒËœÃ‚Â§ÃƒËœÃ‚Â² ÃƒËœÃ‚Â´Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â¯.
/// </summary>
public async Task ResetInstrumentsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<PortfolioTransaction>().ExecuteDeleteAsync();
        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Resets tables used by Listing endpoint scenarios while respecting foreign-key order.
/// FA: ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â¯Ãƒâ„¢Ã‹â€ Ãƒâ„¢Ã¢â‚¬Å¾ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â±ÃƒËœÃ‚Â¯ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒâ„¢Ã‚ÂÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯Ãƒâ„¢Ã¢â‚¬Â¡ ÃƒËœÃ‚Â³Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â§ÃƒËœÃ‚Â±Ãƒâ€ºÃ…â€™Ãƒâ„¢Ã‹â€ Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Listing ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â±ÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Âª ÃƒËœÃ‚ÂªÃƒËœÃ‚Â±ÃƒËœÃ‚ÂªÃƒâ€ºÃ…â€™ÃƒËœÃ‚Â¨ ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Å¾Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â¯Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚Â®ÃƒËœÃ‚Â§ÃƒËœÃ‚Â±ÃƒËœÃ‚Â¬Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã‚Â¾ÃƒËœÃ‚Â§ÃƒÅ¡Ã‚Â©ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â³ÃƒËœÃ‚Â§ÃƒËœÃ‚Â²Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
/// </summary>
public async Task ResetListingsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<PortfolioTransaction>().ExecuteDeleteAsync();
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
/// FA: Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÅ¡Ã‚Â¯Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã‚ÂÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Å¾ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â±ÃƒËœÃ‚Â¯ Ãƒâ„¢Ã¢â‚¬Â Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â§ÃƒËœÃ‚Â² ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Endpoint Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â±ÃƒËœÃ‚Â¨Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â· ÃƒËœÃ‚Â¨Ãƒâ„¢Ã¢â‚¬Â¡ Listing ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
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
/// FA: ÃƒËœÃ‚Â¯ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯Ãƒâ„¢Ã¢â‚¬Â¡ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚Âª ÃƒËœÃ‚ÂªÃƒâ„¢Ã¢â‚¬Å¡Ãƒâ„¢Ã‹â€ Ãƒâ€ºÃ…â€™Ãƒâ„¢Ã¢â‚¬Â¦ Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã¢â‚¬Å¾ÃƒËœÃ‚Â§ÃƒËœÃ‚ÂªÃƒâ€ºÃ…â€™ ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ Ãƒâ„¢Ã‚Â¾Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â´ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â² MarketÃƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Å¾ÃƒËœÃ‚Â¯ Ãƒâ„¢Ã‚Â¾ÃƒËœÃ‚Â§ÃƒÅ¡Ã‚Â©ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â³ÃƒËœÃ‚Â§ÃƒËœÃ‚Â²Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
/// </summary>
public async Task ResetTradingCalendarsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<PortfolioTransaction>().ExecuteDeleteAsync();
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
/// FA: Ãƒâ€ºÃ…â€™ÃƒÅ¡Ã‚Â© Market Ãƒâ„¢Ã‚ÂÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Å¾ ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚ÂªÃƒâ„¢Ã¢â‚¬Å¡Ãƒâ„¢Ã‹â€ Ãƒâ€ºÃ…â€™Ãƒâ„¢Ã¢â‚¬Â¦ Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã¢â‚¬Å¾ÃƒËœÃ‚Â§ÃƒËœÃ‚ÂªÃƒâ€ºÃ…â€™ ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
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
/// <summary>EN: Resets MarketPrice data and Listing dependencies. FA: ÃƒËœÃ‚Â¯ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯Ãƒâ„¢Ã¢â‚¬Â¡ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ MarketPrice Ãƒâ„¢Ã‹â€  Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÅ¡Ã‚Â¯Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Listing ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ Ãƒâ„¢Ã‚Â¾ÃƒËœÃ‚Â§ÃƒÅ¡Ã‚Â©ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â³ÃƒËœÃ‚Â§ÃƒËœÃ‚Â²Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.</summary>
public async Task ResetMarketPricesAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        EnsureSafeTestDatabase(dbContext);
        await dbContext.Set<PortfolioTransaction>().ExecuteDeleteAsync();
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
/// <summary>EN: Creates an active Listing for MarketPrice tests. FA: Ãƒâ€ºÃ…â€™ÃƒÅ¡Ã‚Â© Listing Ãƒâ„¢Ã‚ÂÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Å¾ ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ MarketPrice ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.</summary>
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
/// FA: ÃƒËœÃ‚Â¯ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯Ãƒâ„¢Ã¢â‚¬Â¡ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚Â¢Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â§ÃƒËœÃ‚Â± Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã¢â‚¬Å¾ÃƒËœÃ‚Â§ÃƒËœÃ‚Âª ÃƒËœÃ‚Â±Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â²ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Â Ãƒâ„¢Ã¢â‚¬Â¡ Ãƒâ„¢Ã‹â€  Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÅ¡Ã‚Â¯Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Listing ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ Ãƒâ„¢Ã‚Â¾ÃƒËœÃ‚Â§ÃƒÅ¡Ã‚Â©ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â³ÃƒËœÃ‚Â§ÃƒËœÃ‚Â²Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
/// </summary>
public async Task ResetDailyTradeStatisticsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<PortfolioTransaction>().ExecuteDeleteAsync();
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
/// FA: Ãƒâ€ºÃ…â€™ÃƒÅ¡Ã‚Â© Listing Ãƒâ„¢Ã‚ÂÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Å¾ ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚Â¢Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â§ÃƒËœÃ‚Â± Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã¢â‚¬Å¾ÃƒËœÃ‚Â§ÃƒËœÃ‚Âª ÃƒËœÃ‚Â±Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â²ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Â Ãƒâ„¢Ã¢â‚¬Â¡ ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
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
/// FA: TickÃƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Å¡Ãƒâ€ºÃ…â€™Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Âª ÃƒËœÃ‚Â¯ÃƒËœÃ‚Â±Ãƒâ„¢Ã‹â€ Ãƒâ„¢Ã¢â‚¬Â ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â±Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â²Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã‹â€  Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÅ¡Ã‚Â¯Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Listing ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ Ãƒâ„¢Ã‚Â¾ÃƒËœÃ‚Â§ÃƒÅ¡Ã‚Â©ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â³ÃƒËœÃ‚Â§ÃƒËœÃ‚Â²Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
/// </summary>
public async Task ResetIntradayPriceTicksAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<PortfolioTransaction>().ExecuteDeleteAsync();
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
/// FA: Ãƒâ€ºÃ…â€™ÃƒÅ¡Ã‚Â© Listing Ãƒâ„¢Ã‚ÂÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Å¾ ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Tick Ãƒâ„¢Ã¢â‚¬Å¡Ãƒâ€ºÃ…â€™Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Âª ÃƒËœÃ‚Â¯ÃƒËœÃ‚Â±Ãƒâ„¢Ã‹â€ Ãƒâ„¢Ã¢â‚¬Â ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â±Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â²Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
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
/// FA: Ãƒâ„¢Ã‚Â¾ÃƒËœÃ‚Â³ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â² Ãƒâ„¢Ã‚Â¾ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Â  Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â¬Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â¹Ãƒâ„¢Ã¢â‚¬Â¡ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒËœÃ…â€™ ÃƒËœÃ‚Â¯Ãƒâ€ºÃ…â€™ÃƒËœÃ‚ÂªÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â³ Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â²ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â­ÃƒËœÃ‚Â°Ãƒâ„¢Ã‚Â ÃƒÅ¡Ã‚Â©ÃƒËœÃ‚Â±ÃƒËœÃ‚Â¯Ãƒâ„¢Ã¢â‚¬Â¡ Ãƒâ„¢Ã‹â€  Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â¹ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚Âª ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ ÃƒËœÃ‚Â¢ÃƒËœÃ‚Â²ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.
/// </summary>
/// <summary>EN: Resets OrderBookSnapshot data and Listing dependencies. FA: ÃƒËœÃ‚Â¯ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯Ãƒâ„¢Ã¢â‚¬Â¡ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ OrderBookSnapshot Ãƒâ„¢Ã‹â€  Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÅ¡Ã‚Â¯Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Listing ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ Ãƒâ„¢Ã‚Â¾ÃƒËœÃ‚Â§ÃƒÅ¡Ã‚Â©ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â³ÃƒËœÃ‚Â§ÃƒËœÃ‚Â²Ãƒâ€ºÃ…â€™ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.</summary>
public async Task ResetOrderBookSnapshotsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        EnsureSafeTestDatabase(dbContext);
        await dbContext.Set<PortfolioTransaction>().ExecuteDeleteAsync();
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

/// <summary>EN: Creates an active Listing for OrderBookSnapshot tests. FA: Ãƒâ€ºÃ…â€™ÃƒÅ¡Ã‚Â© Listing Ãƒâ„¢Ã‚ÂÃƒËœÃ‚Â¹ÃƒËœÃ‚Â§Ãƒâ„¢Ã¢â‚¬Å¾ ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ OrderBookSnapshot ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ÃƒËœÃ‚Â¬ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¯ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒÅ¡Ã‚Â©Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â¯.</summary>
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

        await dbContext.Set<PortfolioTransaction>().ExecuteDeleteAsync();
        await dbContext.Set<Portfolio>().ExecuteDeleteAsync();
        await dbContext.Set<Investor>().ExecuteDeleteAsync();
    }
/// <summary>
/// EN: Resets portfolio-transaction test data and all required dependencies in FK-safe order.
/// FA: Ø¯Ø§Ø¯Ù‡â€ŒÙ‡Ø§ÛŒ ØªØ³Øª ØªØ±Ø§Ú©Ù†Ø´ Ù¾Ø±ØªÙÙˆÛŒ Ùˆ ÙˆØ§Ø¨Ø³ØªÚ¯ÛŒâ€ŒÙ‡Ø§ÛŒ Ù„Ø§Ø²Ù… Ø±Ø§ Ø¨Ø§ ØªØ±ØªÛŒØ¨ Ø§Ù…Ù† FK Ù¾Ø§Ú©â€ŒØ³Ø§Ø²ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
/// </summary>
public async Task ResetPortfolioTransactionsAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        EnsureSafeTestDatabase(dbContext);

        await dbContext.Set<PortfolioTransaction>().ExecuteDeleteAsync();
        await dbContext.Set<OrderBookSnapshot>().ExecuteDeleteAsync();
        await dbContext.Set<IntradayPriceTick>().ExecuteDeleteAsync();
        await dbContext.Set<DailyTradeStatistics>().ExecuteDeleteAsync();
        await dbContext.Set<MarketPrice>().ExecuteDeleteAsync();
        await dbContext.Set<Listing>().ExecuteDeleteAsync();
        await dbContext.Set<Portfolio>().ExecuteDeleteAsync();
        await dbContext.Set<Investor>().ExecuteDeleteAsync();
        await dbContext.Set<TradingCalendar>().ExecuteDeleteAsync();
        await dbContext.Set<Venue>().ExecuteDeleteAsync();
        await dbContext.Set<Market>().ExecuteDeleteAsync();
        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
        await dbContext.Set<Currency>().ExecuteDeleteAsync();
    }

/// <summary>
/// EN: Creates an active Listing for portfolio-transaction integration tests.
/// FA: ÛŒÚ© Listing ÙØ¹Ø§Ù„ Ø¨Ø±Ø§ÛŒ ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Integration ØªØ±Ø§Ú©Ù†Ø´ Ù¾Ø±ØªÙÙˆÛŒ Ø§ÛŒØ¬Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
/// </summary>
internal async Task<string> CreatePortfolioTransactionListingAsync()
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
            new MarketBook.Domain.Listing.ValueObjects.TradingSymbol("PTRANS"),
            0.00000001m,
            8);

        await dbContext.Set<Listing>().AddAsync(listing);
        await dbContext.SaveChangesAsync();

        return listing.Id.Value.ToString();
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
/// FA: ÃƒËœÃ‚Â´Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â§ÃƒËœÃ‚Â³Ãƒâ„¢Ã¢â‚¬Â¡ Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â§ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â³ÃƒËœÃ‚ÂªÃƒÅ¡Ã‚Â¯Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™Ãƒâ„¢Ã¢â‚¬Â¡ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ Seed ÃƒËœÃ‚Â´ÃƒËœÃ‚Â¯Ãƒâ„¢Ã¢â‚¬Â¡ ÃƒËœÃ‚Â¨ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚Â³Ãƒâ„¢Ã¢â‚¬Â ÃƒËœÃ‚Â§ÃƒËœÃ‚Â±Ãƒâ€ºÃ…â€™Ãƒâ„¢Ã‹â€ Ãƒâ€ºÃ…â€™ ÃƒËœÃ‚ÂªÃƒËœÃ‚Â³ÃƒËœÃ‚Âª Integration Ãƒâ„¢Ã¢â‚¬Â¦ÃƒËœÃ‚Â±ÃƒËœÃ‚Â¨Ãƒâ„¢Ã‹â€ ÃƒËœÃ‚Â· ÃƒËœÃ‚Â¨Ãƒâ„¢Ã¢â‚¬Â¡ Listing ÃƒËœÃ‚Â±ÃƒËœÃ‚Â§ Ãƒâ„¢Ã¢â‚¬Â ÃƒÅ¡Ã‚Â¯Ãƒâ„¢Ã¢â‚¬Â¡ Ãƒâ„¢Ã¢â‚¬Â¦Ãƒâ€ºÃ…â€™ÃƒÂ¢Ã¢â€šÂ¬Ã…â€™ÃƒËœÃ‚Â¯ÃƒËœÃ‚Â§ÃƒËœÃ‚Â±ÃƒËœÃ‚Â¯.
/// </summary>
internal sealed record ListingTestSeed(
    string InstrumentId,
    string VenueId,
    string QuoteCurrencyId);


