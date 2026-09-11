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
/// EN: Integration tests for portfolio performance chart series.
/// FA: تست‌های Integration مربوط به سری زمانی نمودار عملکرد پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioPerformanceSeriesEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes chart-series endpoint tests.
    /// FA: تست‌های Endpoint سری زمانی نمودار را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioPerformanceSeriesEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies daily sampling includes From and To and exposes cumulative TWR.
    /// FA: بررسی می‌کند نمونه‌برداری روزانه From و To را شامل شده و TWR تجمعی را نمایش دهد.
    /// </summary>
    [Fact]
    public async Task Daily_Series_Should_Include_From_And_To_With_Cumulative_Twr()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("EUR", $"Series EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Series Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Series {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null, null, null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, from.AddDays(1), null, null, "Dividend"));

        using HttpResponseMessage response =
            await GetSeriesAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] points =
            document.RootElement.GetProperty("points").EnumerateArray().ToArray();

        Assert.Equal(3, points.Length);
        Assert.Equal(from, DateTimeOffset.Parse(points[0].GetProperty("timestamp").GetString()!));
        Assert.Equal(to, DateTimeOffset.Parse(points[2].GetProperty("timestamp").GetString()!));
        Assert.Equal(0m, points[0].GetProperty("cumulativeTimeWeightedReturn").GetDecimal());
        Assert.Equal(0.1m, points[1].GetProperty("cumulativeTimeWeightedReturn").GetDecimal());
        Assert.Equal(0.1m, points[2].GetProperty("cumulativeTimeWeightedReturn").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies weekly sampling appends a non-aligned exact To boundary.
    /// FA: بررسی می‌کند نمونه‌برداری هفتگی در صورت هم‌تراز نبودن، To دقیق را اضافه کند.
    /// </summary>
    [Fact]
    public async Task Weekly_Series_Should_Append_NonAligned_To()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("GBP", $"Series GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Weekly Series Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Weekly Series {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 1, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 18, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null, null, null));

        using HttpResponseMessage response =
            await GetSeriesAsync(portfolioId, from, to, "Weekly");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] points =
            document.RootElement.GetProperty("points").EnumerateArray().ToArray();

        Assert.Equal(4, points.Length);
        Assert.Equal(from.AddDays(7), DateTimeOffset.Parse(points[1].GetProperty("timestamp").GetString()!));
        Assert.Equal(from.AddDays(14), DateTimeOffset.Parse(points[2].GetProperty("timestamp").GetString()!));
        Assert.Equal(to, DateTimeOffset.Parse(points[3].GetProperty("timestamp").GetString()!));
    }

    /// <summary>
    /// EN: Verifies external Deposit markers remain visible while cumulative TWR neutralizes the capital flow.
    /// FA: بررسی می‌کند marker واریز خارجی نمایش داده شود و TWR تجمعی اثر سرمایه ورودی را خنثی کند.
    /// </summary>
    [Fact]
    public async Task Series_Should_Expose_External_Flow_Marker_And_Neutral_Twr()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("CHF", $"Series CHF {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Flow Series Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Flow Series {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset flowTime = from.AddDays(1);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null, null, null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 500m, flowTime, null, null, null));

        using HttpResponseMessage response =
            await GetSeriesAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] flows =
            document.RootElement.GetProperty("externalFlows").EnumerateArray().ToArray();

        Assert.Single(flows);
        Assert.Equal("Deposit", flows[0].GetProperty("type").GetString());
        Assert.Equal(500m, flows[0].GetProperty("signedSourceAmount").GetDecimal());
        Assert.True(flows[0].GetProperty("isFxAvailable").GetBoolean());

        JsonElement finalPoint =
            document.RootElement.GetProperty("points").EnumerateArray().Last();

        Assert.Equal(0m, finalPoint.GetProperty("cumulativeTimeWeightedReturn").GetDecimal());
    }

    private async Task<HttpResponseMessage> GetSeriesAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/series" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}");

    private async Task<string> CreateCurrencyAsync(string code, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/currencies",
                new CurrencyRequest(code, name, 2));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task<string> CreateInvestorAsync(string fullName)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/investors",
                new InvestorRequest(fullName));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task<string> CreatePortfolioAsync(string investorId, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(investorId, name));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
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
