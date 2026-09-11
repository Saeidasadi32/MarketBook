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
/// EN: Integration tests for Sharpe and Sortino risk ratios.
/// FA: تست‌های Integration نسبت‌های ریسک Sharpe و Sortino.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioRiskRatiosEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes risk-ratio endpoint tests.
    /// FA: تست‌های Endpoint نسبت‌های ریسک را مقداردهی می‌کند.
    /// </summary>
    public PortfolioRiskRatiosEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies symmetric +10% and -10% returns produce zero Sharpe and Sortino.
    /// FA: بررسی می‌کند بازده‌های متقارن +10% و -10% نسبت‌های Sharpe و Sortino صفر تولید کنند.
    /// </summary>
    [Fact]
    public async Task RiskRatios_Should_Be_Zero_When_Mean_Return_Is_Zero()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("EUR", $"Ratios EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Ratios Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Ratios {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset dayOne = from.AddDays(1);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, dayOne, "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 110m, to, "Fee"));

        using HttpResponseMessage response =
            await GetRiskRatiosAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.True(root.GetProperty("isSharpeCalculable").GetBoolean());
        Assert.True(root.GetProperty("isSortinoCalculable").GetBoolean());
        Assert.Equal(0m, root.GetProperty("riskFreeRateAnnual").GetDecimal());
        Assert.Equal(0m, root.GetProperty("minimumAcceptableReturnAnnual").GetDecimal());
        Assert.InRange(root.GetProperty("sharpeRatio").GetDecimal(), -0.000001m, 0.000001m);
        Assert.InRange(root.GetProperty("sortinoRatio").GetDecimal(), -0.000001m, 0.000001m);
    }

    /// <summary>
    /// EN: Verifies positive mean with one negative observation produces positive Sharpe and Sortino.
    /// FA: بررسی می‌کند میانگین مثبت همراه با یک مشاهده منفی، Sharpe و Sortino مثبت تولید کند.
    /// </summary>
    [Fact]
    public async Task RiskRatios_Should_Calculate_Positive_Sharpe_And_Sortino()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("GBP", $"Ratios GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Ratios Positive Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Ratios Positive {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset dayOne = from.AddDays(1);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, dayOne, "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 55m, to, "Fee"));

        using HttpResponseMessage response =
            await GetRiskRatiosAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isSharpeCalculable").GetBoolean());
        Assert.True(root.GetProperty("isSortinoCalculable").GetBoolean());

        decimal sharpe = root.GetProperty("sharpeRatio").GetDecimal();
        decimal sortino = root.GetProperty("sortinoRatio").GetDecimal();

        decimal annualization = (decimal)Math.Sqrt(365d);
        decimal expectedSharpe =
            (0.025m / (decimal)Math.Sqrt(0.01125d)) *
            annualization;

        decimal expectedSortino =
            (0.025m / (decimal)Math.Sqrt(0.00125d)) *
            annualization;

        Assert.InRange(
            sharpe,
            expectedSharpe - 0.0001m,
            expectedSharpe + 0.0001m);

        Assert.InRange(
            sortino,
            expectedSortino - 0.0001m,
            expectedSortino + 0.0001m);
    }

    /// <summary>
    /// EN: Verifies zero volatility and zero downside deviation return null ratios rather than infinities.
    /// FA: بررسی می‌کند نوسان و انحراف نزولی صفر به‌جای بی‌نهایت، نسبت null برگردانند.
    /// </summary>
    [Fact]
    public async Task RiskRatios_Should_Not_Fabricate_Ratios_When_Denominators_Are_Zero()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("CHF", $"Ratios CHF {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Ratios Flat Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Ratios Flat {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));

        using HttpResponseMessage response =
            await GetRiskRatiosAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.False(root.GetProperty("isSharpeCalculable").GetBoolean());
        Assert.False(root.GetProperty("isSortinoCalculable").GetBoolean());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("sharpeRatio").ValueKind);
        Assert.Equal(JsonValueKind.Null, root.GetProperty("sortinoRatio").ValueKind);
    }

    private async Task<HttpResponseMessage> GetRiskRatiosAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/risk-ratios" +
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
        string? Description);
}
