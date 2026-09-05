// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Infrastructure
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MarketBook.Integration.Tests.Infrastructure;

/// <summary>
/// EN: Hosts the MarketBook API in memory and redirects persistence to an isolated integration-test database.
/// FA: API مارکت‌بوک را در حافظه اجرا کرده و Persistence را به دیتابیس مجزای تست Integration هدایت می‌کند.
/// </summary>
public sealed class MarketBookApiFactory : WebApplicationFactory<Program>
{
    internal const string TestDatabasePrefix = "MarketBookIntegrationTestsDb_";

    private static readonly string TestDatabaseName =
        $"{TestDatabasePrefix}{Environment.ProcessId}_{Guid.NewGuid():N}";

    /// <summary>
    /// EN: Configures the test host and replaces only the database name in the application's connection string.
    /// FA: میزبان تست را پیکربندی کرده و فقط نام دیتابیس را در Connection String برنامه جایگزین می‌کند.
    /// </summary>
    /// <param name="builder">
    /// EN: Web host builder used by the test server.
    /// FA: سازنده میزبان وب مورد استفاده Test Server.
    /// </param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        string repositoryRoot = FindRepositoryRoot();
        string apiContentRoot = Path.Combine(repositoryRoot, "src", "MarketBook.Api");

        builder.UseContentRoot(apiContentRoot);
        builder.UseEnvironment("IntegrationTests");

        string? testConnectionString = null;

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            IConfigurationRoot configuration = configurationBuilder.Build();

            string connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not configured for integration tests.");

            testConnectionString =
                BuildTestConnectionString(connectionString);

            Dictionary<string, string?> overrides = new()
            {
                ["ConnectionStrings:DefaultConnection"] = testConnectionString
            };

            configurationBuilder.AddInMemoryCollection(overrides);
        });

        builder.ConfigureServices(services =>
        {
            if (string.IsNullOrWhiteSpace(testConnectionString))
            {
                throw new InvalidOperationException(
                    "The integration-test connection string was not initialized.");
            }

            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(testConnectionString));
        });
    }

    internal static bool IsIntegrationTestDatabase(string databaseName)
    {
        return !string.IsNullOrWhiteSpace(databaseName) &&
               databaseName.StartsWith(
                   TestDatabasePrefix,
                   StringComparison.OrdinalIgnoreCase);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            string solutionPath = Path.Combine(directory.FullName, "MarketBook.slnx");

            if (File.Exists(solutionPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate the MarketBook repository root containing MarketBook.slnx.");
    }

    private static string BuildTestConnectionString(string connectionString)
    {
        const string databasePattern =
            @"(?i)(Database|Initial\s+Catalog)\s*=\s*[^;]*";

        if (Regex.IsMatch(connectionString, databasePattern))
        {
            return Regex.Replace(
                connectionString,
                databasePattern,
                $"Database={TestDatabaseName}");
        }

        string separator =
            connectionString.EndsWith(';') ? string.Empty : ";";

        return $"{connectionString}{separator}Database={TestDatabaseName};";
    }
}
