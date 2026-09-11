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
/// EN: Integration tests for rolling risk analytics.
/// FA: تست‌های Integration تحلیل ریسک Rolling.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioRollingRiskEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes rolling-risk endpoint tests.
    /// FA: تست‌های Endpoint ریسک Rolling را مقداردهی می‌کند.
    /// </summary>
    public PortfolioRollingRiskEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies a two-observation rolling window emits the expected volatility and ratios.
    /// FA: بررسی می‌کند پنجره Rolling دو مشاهده‌ای، نوسان و نسبت‌های مورد انتظار را تولید کند.
    /// </summary>
    [Fact]
    public async Task RollingRisk_Should_Calculate_First_Full_Window()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("EUR", $"Rolling EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Rolling Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Rolling {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset dayOne = from.AddDays(1);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, dayOne, "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 110m, to, "Fee"));

        using HttpResponseMessage response =
            await GetRollingRiskAsync(
                portfolioId,
                from,
                to,
                "Daily",
                2,
                0m,
                0m);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(1, root.GetProperty("pointCount").GetInt32());

        JsonElement point = root.GetProperty("points")[0];

        Assert.Equal(2, point.GetProperty("observationCount").GetInt32());
        Assert.InRange(
            point.GetProperty("meanPeriodicReturn").GetDecimal(),
            -0.0000001m,
            0.0000001m);

        decimal expectedVolatility =
            (decimal)Math.Sqrt(0.02d);

        decimal expectedDownside =
            (decimal)Math.Sqrt(0.005d);

        Assert.InRange(
            point.GetProperty("periodicVolatility").GetDecimal(),
            expectedVolatility - 0.000001m,
            expectedVolatility + 0.000001m);

        Assert.InRange(
            point.GetProperty("periodicDownsideDeviation").GetDecimal(),
            expectedDownside - 0.000001m,
            expectedDownside + 0.000001m);
    }

    /// <summary>
    /// EN: Verifies a three-return series with window two emits exactly two rolling points.
    /// FA: بررسی می‌کند سری سه بازدهی با پنجره دو، دقیقاً دو نقطه Rolling تولید کند.
    /// </summary>
    [Fact]
    public async Task RollingRisk_Should_Emit_One_Point_Per_Full_Sliding_Window()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("GBP", $"Rolling GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Rolling Windows Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Rolling Windows {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(3);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, from.AddDays(1), "Dividend1"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 55m, from.AddDays(2), "Fee"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 52.25m, to, "Dividend2"));

        using HttpResponseMessage response =
            await GetRollingRiskAsync(
                portfolioId,
                from,
                to,
                "Daily",
                2,
                0m,
                0m);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.Equal(2, root.GetProperty("pointCount").GetInt32());
        Assert.Equal(2, root.GetProperty("points").GetArrayLength());
    }

    /// <summary>
    /// EN: Verifies an oversized warm-up window returns no partial rolling statistic.
    /// FA: بررسی می‌کند پنجره بزرگ‌تر از داده موجود، آمار Rolling ناقص تولید نکند.
    /// </summary>
    [Fact]
    public async Task RollingRisk_Should_Not_Emit_Partial_Warmup_Window()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("CHF", $"Rolling CHF {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Rolling Warmup Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Rolling Warmup {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, from.AddDays(1), "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 110m, to, "Fee"));

        using HttpResponseMessage response =
            await GetRollingRiskAsync(
                portfolioId,
                from,
                to,
                "Daily",
                3,
                0m,
                0m);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(0, root.GetProperty("pointCount").GetInt32());
        Assert.Equal(0, root.GetProperty("points").GetArrayLength());
    }

    private async Task<HttpResponseMessage> GetRollingRiskAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval,
        int windowPeriods,
        decimal riskFreeRateAnnual,
        decimal minimumAcceptableReturnAnnual)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/risk/rolling" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}" +
            $"&windowPeriods={windowPeriods}" +
            $"&riskFreeRateAnnual={riskFreeRateAnnual.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
            $"&minimumAcceptableReturnAnnual={minimumAcceptableReturnAnnual.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

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
        string? Description);
}
