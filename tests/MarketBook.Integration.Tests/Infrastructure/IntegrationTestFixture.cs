// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Infrastructure
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Instrument.Aggregates;
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

        await dbContext.Set<Instrument>().ExecuteDeleteAsync();
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
