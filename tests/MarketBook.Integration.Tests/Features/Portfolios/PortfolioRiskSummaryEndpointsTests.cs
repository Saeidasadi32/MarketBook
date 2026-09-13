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
/// EN: Integration tests for the compact portfolio risk dashboard.
/// FA: تست‌های Integration داشبورد فشرده ریسک پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioRiskSummaryEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes risk-summary endpoint tests.
    /// FA: تست‌های Endpoint خلاصه ریسک را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioRiskSummaryEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies the dashboard composes core risk metrics consistently.
    /// FA: بررسی می‌کند داشبورد سنجه‌های اصلی ریسک را به‌صورت سازگار ترکیب کند.
    /// </summary>
    [Fact]
    public async Task RiskSummary_Should_Compose_Core_Risk_Metrics()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("ISK", $"Risk Summary ISK {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Risk Summary Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Risk Summary {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset day1 = from.AddDays(1);
        DateTimeOffset day2 = from.AddDays(2);
        DateTimeOffset to = from.AddDays(3);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, day1, "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 100m, day2, "Fee"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 50m, to, "Fee"));

        using HttpResponseMessage response =
            await GetRiskSummaryAsync(portfolioId, from, to, "Daily", 0.95m, 0m, 0m);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.Equal(portfolioId, root.GetProperty("portfolioId").GetString());
        Assert.Equal(currencyId, root.GetProperty("baseCurrencyId").GetString());
        Assert.True(root.GetProperty("isComplete").GetBoolean());

        JsonElement statistics = root.GetProperty("riskStatistics");
        Assert.True(statistics.GetProperty("isComplete").GetBoolean());
        Assert.True(statistics.GetProperty("isCalculable").GetBoolean());
        Assert.Equal(3, statistics.GetProperty("observationCount").GetInt32());
        Assert.True(statistics.GetProperty("annualizedVolatility").GetDecimal() > 0m);

        JsonElement valueAtRisk = root.GetProperty("valueAtRisk");
        Assert.True(valueAtRisk.GetProperty("isComplete").GetBoolean());
        Assert.True(valueAtRisk.GetProperty("isCalculable").GetBoolean());
        Assert.Equal(950m, valueAtRisk.GetProperty("netAssetValueBase").GetDecimal());
        Assert.True(valueAtRisk.GetProperty("valueAtRiskAmountBase").GetDecimal() > 0m);

        JsonElement drawdown = root.GetProperty("drawdown");
        Assert.True(drawdown.GetProperty("isComplete").GetBoolean());
        Assert.Equal(150m, drawdown.GetProperty("currentDrawdownAmountBase").GetDecimal());
        Assert.Equal(150m, drawdown.GetProperty("maximumDrawdownAmountBase").GetDecimal());

        JsonElement episodes = root.GetProperty("drawdownEpisodes");
        Assert.True(episodes.GetProperty("isComplete").GetBoolean());
        Assert.True(episodes.GetProperty("hasActiveDrawdown").GetBoolean());
        Assert.Equal(1, episodes.GetProperty("episodeCount").GetInt32());
        Assert.Equal(150m, episodes.GetProperty("activeEpisodeCurrentDrawdownAmountBase").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies dashboard configuration is forwarded to composed projections.
    /// FA: بررسی می‌کند تنظیمات داشبورد به projectionهای ترکیبی منتقل شوند.
    /// </summary>
    [Fact]
    public async Task RiskSummary_Should_Forward_Risk_Configuration()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("PLN", $"Risk Summary PLN {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Risk Summary Config Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Risk Summary Config {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(3);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 50m, from.AddDays(1), "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 25m, from.AddDays(2), "Fee"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 10m, to, "Dividend"));

        using HttpResponseMessage response =
            await GetRiskSummaryAsync(portfolioId, from, to, "Daily", 0.99m, 0.05m, 0.03m);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        JsonElement ratios = root.GetProperty("riskRatios");
        Assert.Equal(0.05m, ratios.GetProperty("riskFreeRateAnnual").GetDecimal());
        Assert.Equal(0.03m, ratios.GetProperty("minimumAcceptableReturnAnnual").GetDecimal());

        JsonElement valueAtRisk = root.GetProperty("valueAtRisk");
        Assert.Equal(0.99m, valueAtRisk.GetProperty("confidenceLevel").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies invalid historical confidence is rejected by the composed VaR contract.
    /// FA: بررسی می‌کند سطح اطمینان تاریخی نامعتبر توسط قرارداد VaR ترکیبی رد شود.
    /// </summary>
    [Fact]
    public async Task RiskSummary_Should_Reject_Invalid_Confidence_Level()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("SEK", $"Risk Summary SEK {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Risk Summary Invalid Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Risk Summary Invalid {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, currencyId);

        using HttpResponseMessage response =
            await GetRiskSummaryAsync(portfolioId, from, from.AddDays(2), "Daily", 1m, 0m, 0m);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HttpResponseMessage> GetRiskSummaryAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval,
        decimal confidenceLevel,
        decimal riskFreeRateAnnual,
        decimal minimumAcceptableReturnAnnual)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/risk/summary" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}" +
            $"&confidenceLevel={confidenceLevel}" +
            $"&riskFreeRateAnnual={riskFreeRateAnnual}" +
            $"&minimumAcceptableReturnAnnual={minimumAcceptableReturnAnnual}");

    private async Task<string> CreateCurrencyAsync(string code, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync("/api/v1/currencies", new CurrencyRequest(code, name, 2));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);

        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task<string> CreateInvestorAsync(string fullName)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync("/api/v1/investors", new InvestorRequest(fullName));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);

        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task<string> CreatePortfolioAsync(string investorId, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync("/api/v1/portfolios", new PortfolioRequest(investorId, name));

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
            await _client.PostAsJsonAsync("/api/v1/portfolio-cash-transactions", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

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
