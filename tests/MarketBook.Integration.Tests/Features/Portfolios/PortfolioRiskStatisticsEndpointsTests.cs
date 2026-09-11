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
/// EN: Integration tests for volatility and downside-deviation statistics.
/// FA: تست‌های Integration آمار نوسان و انحراف نزولی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioRiskStatisticsEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes risk-statistics endpoint tests.
    /// FA: تست‌های Endpoint آمار ریسک را مقداردهی می‌کند.
    /// </summary>
    public PortfolioRiskStatisticsEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies +10% and -10% daily returns produce the expected sample volatility and downside deviation.
    /// FA: بررسی می‌کند بازده‌های روزانه +10% و -10% نوسان نمونه و انحراف نزولی مورد انتظار را تولید کنند.
    /// </summary>
    [Fact]
    public async Task RiskStatistics_Should_Calculate_Volatility_And_Downside_Deviation()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("EUR", $"Risk EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Risk Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Risk {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset dayOne = from.AddDays(1);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, dayOne, "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 110m, to, "Fee"));

        using HttpResponseMessage response =
            await GetRiskStatisticsAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.True(root.GetProperty("isCalculable").GetBoolean());
        Assert.Equal(2, root.GetProperty("observationCount").GetInt32());
        Assert.Equal(365, root.GetProperty("annualizationPeriodsPerYear").GetInt32());
        Assert.InRange(root.GetProperty("meanPeriodicReturn").GetDecimal(), -0.0000001m, 0.0000001m);

        decimal periodicVolatility =
            root.GetProperty("periodicVolatility").GetDecimal();

        decimal periodicDownsideDeviation =
            root.GetProperty("periodicDownsideDeviation").GetDecimal();

        decimal expectedVolatility =
            (decimal)Math.Sqrt(0.02d);

        decimal expectedDownsideDeviation =
            (decimal)Math.Sqrt(0.005d);

        Assert.InRange(
            periodicVolatility,
            expectedVolatility - 0.000001m,
            expectedVolatility + 0.000001m);

        Assert.InRange(
            periodicDownsideDeviation,
            expectedDownsideDeviation - 0.000001m,
            expectedDownsideDeviation + 0.000001m);
    }

    /// <summary>
    /// EN: Verifies all-positive periodic returns have zero downside deviation.
    /// FA: بررسی می‌کند بازده‌های دوره‌ای کاملاً مثبت انحراف نزولی صفر داشته باشند.
    /// </summary>
    [Fact]
    public async Task RiskStatistics_Should_Have_Zero_Downside_When_All_Returns_Are_Positive()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("GBP", $"Risk GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Risk Positive Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Risk Positive {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, from.AddDays(1), "Dividend1"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 110m, to, "Dividend2"));

        using HttpResponseMessage response =
            await GetRiskStatisticsAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isCalculable").GetBoolean());
        Assert.Equal(0m, root.GetProperty("periodicDownsideDeviation").GetDecimal());
        Assert.Equal(0m, root.GetProperty("annualizedDownsideDeviation").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies Deposit and Withdrawal do not create periodic investment volatility.
    /// FA: بررسی می‌کند Deposit و Withdrawal به‌تنهایی نوسان بازده سرمایه‌گذاری ایجاد نکنند.
    /// </summary>
    [Fact]
    public async Task RiskStatistics_Should_Ignore_External_Capital_Flows()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("CHF", $"Risk CHF {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Risk Flow Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Risk Flow {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 2, 200m, from.AddDays(1), "Withdrawal"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 400m, to, "Deposit"));

        using HttpResponseMessage response =
            await GetRiskStatisticsAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.True(root.GetProperty("isCalculable").GetBoolean());
        Assert.Equal(0m, root.GetProperty("periodicVolatility").GetDecimal());
        Assert.Equal(0m, root.GetProperty("annualizedVolatility").GetDecimal());
        Assert.Equal(0m, root.GetProperty("periodicDownsideDeviation").GetDecimal());
    }

    private async Task<HttpResponseMessage> GetRiskStatisticsAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/risk-statistics" +
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
