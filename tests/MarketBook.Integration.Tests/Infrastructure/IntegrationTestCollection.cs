// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Infrastructure
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Integration.Tests.Infrastructure;

/// <summary>
/// EN: Defines the shared integration-test collection and prevents database tests from running in parallel.
/// FA: مجموعه مشترک تست‌های Integration را تعریف کرده و اجرای موازی تست‌های دیتابیس را متوقف می‌کند.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class IntegrationTestCollection
    : ICollectionFixture<IntegrationTestFixture>
{
    /// <summary>
    /// EN: Gets the xUnit collection name used by MarketBook integration tests.
    /// FA: نام Collection مورد استفاده تست‌های Integration مارکت‌بوک را دریافت می‌کند.
    /// </summary>
    public const string Name = "MarketBook Integration Tests";
}
