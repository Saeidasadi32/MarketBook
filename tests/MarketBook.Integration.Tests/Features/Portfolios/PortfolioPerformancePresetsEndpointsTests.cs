// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.Portfolios
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.Portfolios;

/// <summary>
/// EN: Integration tests for standard portfolio performance-period presets.
/// FA: تست‌های Integration مربوط به بازه‌های استاندارد عملکرد پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioPerformancePresetsEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes performance-preset endpoint tests.
    /// FA: تست‌های Endpoint بازه‌های استاندارد عملکرد را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioPerformancePresetsEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies all six stable dashboard presets are returned in deterministic order and end at the supplied as-of instant.
    /// FA: بررسی می‌کند هر شش بازه پایدار داشبورد با ترتیب قطعی و پایان برابر as-of داده‌شده برگردند.
    /// </summary>
    [Fact]
    public async Task Presets_Should_Return_Six_Stable_Periods_In_Order()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("EUR", $"Presets EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Presets Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Presets {Guid.NewGuid():N}");

        await SetBaseCurrencyAsync(portfolioId, currencyId);

        DateTimeOffset asOf = DateTimeOffset.UtcNow.AddYears(2);

        using HttpResponseMessage response =
            await GetPresetsAsync(portfolioId, asOf);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] periods =
            document.RootElement.GetProperty("periods").EnumerateArray().ToArray();

        Assert.Equal(6, periods.Length);

        string?[] expected =
        [
            "1M",
            "3M",
            "6M",
            "YTD",
            "1Y",
            "SinceInception"
        ];

        Assert.Equal(
            expected,
            periods.Select(item => item.GetProperty("preset").GetString()).ToArray());

        foreach (JsonElement period in periods)
        {
            DateTimeOffset to =
                DateTimeOffset.Parse(period.GetProperty("to").GetString()!);

            Assert.Equal(asOf, to);
        }
    }

    /// <summary>
    /// EN: Verifies calendar boundaries for 1M, 3M, 6M, YTD, and 1Y preserve the supplied as-of offset policy.
    /// FA: مرزهای تقویمی 1M، 3M، 6M، YTD و 1Y و سیاست offset مربوط به as-of را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Presets_Should_Resolve_Calendar_Boundaries_From_AsOf()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("GBP", $"Preset Boundary GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Preset Boundary Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Preset Boundary {Guid.NewGuid():N}");

        await SetBaseCurrencyAsync(portfolioId, currencyId);

        DateTimeOffset asOf =
            new(2028, 9, 11, 15, 30, 0, TimeSpan.FromHours(3.5));

        using HttpResponseMessage response =
            await GetPresetsAsync(portfolioId, asOf);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] periods =
            document.RootElement.GetProperty("periods").EnumerateArray().ToArray();

        Dictionary<string, DateTimeOffset> fromByPreset =
            periods.ToDictionary(
                item => item.GetProperty("preset").GetString()!,
                item => DateTimeOffset.Parse(item.GetProperty("from").GetString()!),
                StringComparer.Ordinal);

        Assert.Equal(asOf.AddMonths(-1), fromByPreset["1M"]);
        Assert.Equal(asOf.AddMonths(-3), fromByPreset["3M"]);
        Assert.Equal(asOf.AddMonths(-6), fromByPreset["6M"]);
        Assert.Equal(asOf.AddYears(-1), fromByPreset["1Y"]);

        DateTimeOffset expectedYtd =
            new(2028, 1, 1, 0, 0, 0, TimeSpan.FromHours(3.5));

        Assert.Equal(expectedYtd, fromByPreset["YTD"]);
    }

    /// <summary>
    /// EN: Verifies a one-year preset reuses the existing comparison layer and exposes both returns when the period is fully calculable.
    /// FA: بررسی می‌کند بازه 1Y از لایه مقایسه موجود استفاده کرده و در دوره کاملاً قابل‌محاسبه هر دو بازده را نمایش دهد.
    /// </summary>
    [Fact]
    public async Task OneYear_Preset_Should_Expose_Composed_Twr_And_Xirr()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("CHF", $"Preset Return CHF {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Preset Return Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Preset Return {Guid.NewGuid():N}");

        await SetBaseCurrencyAsync(portfolioId, currencyId);

        DateTimeOffset asOf = DateTimeOffset.UtcNow.AddYears(2);
        DateTimeOffset oneYearStart = asOf.AddYears(-1);

        await CreateCashAsync(new CashRequest(
            portfolioId,
            currencyId,
            1,
            1000m,
            oneYearStart.AddDays(-1),
            null,
            null,
            null));

        await CreateCashAsync(new CashRequest(
            portfolioId,
            currencyId,
            7,
            100m,
            asOf.AddDays(-1),
            null,
            null,
            "Dividend"));

        using HttpResponseMessage response =
            await GetPresetsAsync(portfolioId, asOf);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement oneYear =
            document.RootElement
                .GetProperty("periods")
                .EnumerateArray()
                .Single(item => item.GetProperty("preset").GetString() == "1Y");

        Assert.True(oneYear.GetProperty("isDataComplete").GetBoolean());
        Assert.True(oneYear.GetProperty("areBothReturnsAvailable").GetBoolean());

        decimal twr = oneYear.GetProperty("timeWeightedReturn").GetDecimal();
        decimal xirr = oneYear.GetProperty("moneyWeightedReturn").GetDecimal();

Assert.InRange(twr, 0.0999m, 0.1001m);

double elapsedDays =
    (asOf - oneYearStart).TotalDays;

decimal expectedXirr =
    (decimal)(
        Math.Pow(
            1.1d,
            365.0d / elapsedDays) -
        1.0d);

Assert.InRange(
    xirr,
    expectedXirr - 0.000001m,
    expectedXirr + 0.000001m);
    }

    private async Task<HttpResponseMessage> GetPresetsAsync(
        string portfolioId,
        DateTimeOffset asOf)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/presets" +
            $"?asOf={Uri.EscapeDataString(asOf.ToString("O"))}");

    private async Task<string> CreateCurrencyAsync(string code, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/currencies",
                new CurrencyRequest(code, name, 2));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> CreateInvestorAsync(string fullName)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/investors",
                new InvestorRequest(fullName));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> CreatePortfolioAsync(string investorId, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(investorId, name));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task SetBaseCurrencyAsync(string portfolioId, string currencyId)
    {
        using HttpResponseMessage response =
            await _client.PatchAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/base-currency",
                new SetBaseCurrencyRequest(currencyId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task CreateCashAsync(CashRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-cash-transactions",
                request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record CurrencyRequest(string Code, string Name, int DecimalPlaces);
    private sealed record InvestorRequest(string FullName);
    private sealed record PortfolioRequest(string InvestorId, string Name);
    private sealed record SetBaseCurrencyRequest(string CurrencyId);
    private sealed record CashRequest(
        string PortfolioId,
        string CurrencyId,
        int Type,
        decimal Amount,
        DateTimeOffset OccurredOn,
        string? ReferenceType,
        string? ReferenceId,
        string? Description);
}
