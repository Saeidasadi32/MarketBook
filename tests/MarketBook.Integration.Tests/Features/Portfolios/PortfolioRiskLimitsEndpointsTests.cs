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
/// EN: Integration tests for request-scoped portfolio risk-limit evaluation.
/// FA: تست‌های Integration ارزیابی request-scoped حدود ریسک پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioRiskLimitsEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes portfolio risk-limit endpoint tests.
    /// FA: تست‌های Endpoint حدود ریسک پرتفوی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioRiskLimitsEndpointsTests(
        IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies lenient configured limits remain within limit.
    /// FA: بررسی می‌کند Limitهای آسان پیکربندی‌شده در محدوده باقی بمانند.
    /// </summary>
    [Fact]
    public async Task RiskLimits_Should_Report_WithinLimit()
    {
        PortfolioScenario scenario =
            await CreateScenarioAsync();

        using HttpResponseMessage response =
            await GetRiskLimitsAsync(
                scenario.PortfolioId,
                scenario.From,
                scenario.To,
                "&maxAnnualizedVolatility=10" +
                "&maxValueAtRiskAmountBase=1000" +
                "&maxDrawdownAmountBase=1000");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root =
            document.RootElement;

        Assert.True(
            root.GetProperty("isComplete").GetBoolean());

        Assert.Equal(
            "WithinLimit",
            root.GetProperty("overallStatus").GetString());

        Assert.Equal(
            3,
            root.GetProperty("configuredLimitCount").GetInt32());

        Assert.Equal(
            0,
            root.GetProperty("breachedLimitCount").GetInt32());
    }

    /// <summary>
    /// EN: Verifies a strict monetary drawdown limit is reported as breached with positive exceedance.
    /// FA: بررسی می‌کند حد سخت‌گیرانه Drawdown مبلغی به‌عنوان Breach با میزان عبور مثبت گزارش شود.
    /// </summary>
    [Fact]
    public async Task RiskLimits_Should_Report_Breached_Drawdown_Limit()
    {
        PortfolioScenario scenario =
            await CreateScenarioAsync();

        using HttpResponseMessage response =
            await GetRiskLimitsAsync(
                scenario.PortfolioId,
                scenario.From,
                scenario.To,
                "&maxDrawdownAmountBase=50");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root =
            document.RootElement;

        Assert.Equal(
            "Breached",
            root.GetProperty("overallStatus").GetString());

        Assert.Equal(
            1,
            root.GetProperty("configuredLimitCount").GetInt32());

        Assert.Equal(
            1,
            root.GetProperty("breachedLimitCount").GetInt32());

        JsonElement breachedRule =
            root.GetProperty("rules")
                .EnumerateArray()
                .Single(
                    rule =>
                        rule.GetProperty("code").GetString() ==
                        "MaximumDrawdownAmountBase");

        Assert.Equal(
            "Breached",
            breachedRule.GetProperty("status").GetString());

        Assert.Equal(
            150m,
            breachedRule.GetProperty("actual").GetDecimal());

        Assert.Equal(
            50m,
            breachedRule.GetProperty("limit").GetDecimal());

        Assert.Equal(
            100m,
            breachedRule.GetProperty("breachAmount").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies no supplied limits yields an explicit no-policy status rather than a fabricated pass.
    /// FA: بررسی می‌کند نبود Limit صریحاً NoLimitsConfigured باشد و Pass ساختگی تولید نشود.
    /// </summary>
    [Fact]
    public async Task RiskLimits_Should_Report_NoLimitsConfigured()
    {
        PortfolioScenario scenario =
            await CreateScenarioAsync();

        using HttpResponseMessage response =
            await GetRiskLimitsAsync(
                scenario.PortfolioId,
                scenario.From,
                scenario.To,
                string.Empty);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root =
            document.RootElement;

        Assert.True(
            root.GetProperty("isComplete").GetBoolean());

        Assert.Equal(
            "NoLimitsConfigured",
            root.GetProperty("overallStatus").GetString());

        Assert.Equal(
            0,
            root.GetProperty("configuredLimitCount").GetInt32());

        Assert.Equal(
            7,
            root.GetProperty("rules").GetArrayLength());
    }

    private async Task<PortfolioScenario> CreateScenarioAsync()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "CAD",
                $"Risk Limits CAD {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Risk Limits Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Risk Limits {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        DateTimeOffset day1 =
            from.AddDays(1);

        DateTimeOffset day2 =
            from.AddDays(2);

        DateTimeOffset to =
            from.AddDays(3);

        await SetBaseCurrencyAsync(
            portfolioId,
            currencyId);

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                1,
                1000m,
                from.AddDays(-1),
                null));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                7,
                100m,
                day1,
                "Dividend"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                5,
                100m,
                day2,
                "Fee"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                5,
                50m,
                to,
                "Fee"));

        return new PortfolioScenario(
            portfolioId,
            from,
            to);
    }

    private async Task<HttpResponseMessage> GetRiskLimitsAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string extraQuery)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/risk/limits/evaluate" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            "&interval=Daily" +
            "&confidenceLevel=0.95" +
            "&riskFreeRateAnnual=0" +
            "&minimumAcceptableReturnAnnual=0" +
            extraQuery);

    private async Task<string> CreateCurrencyAsync(
        string code,
        string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/currencies",
                new CurrencyRequest(
                    code,
                    name,
                    2));

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        return document.RootElement
            .GetProperty("id")
            .GetString()!;
    }

    private async Task<string> CreateInvestorAsync(
        string fullName)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/investors",
                new InvestorRequest(fullName));

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        return document.RootElement
            .GetProperty("id")
            .GetString()!;
    }

    private async Task<string> CreatePortfolioAsync(
        string investorId,
        string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(
                    investorId,
                    name));

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        return document.RootElement
            .GetProperty("id")
            .GetString()!;
    }

    private async Task SetBaseCurrencyAsync(
        string portfolioId,
        string currencyId)
    {
        using HttpResponseMessage response =
            await _client.PatchAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/base-currency",
                new SetBaseCurrencyRequest(currencyId));

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    private async Task CreateCashAsync(
        CashRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-cash-transactions",
                request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record PortfolioScenario(
        string PortfolioId,
        DateTimeOffset From,
        DateTimeOffset To);

    private sealed record CurrencyRequest(
        string Code,
        string Name,
        int DecimalPlaces);

    private sealed record InvestorRequest(
        string FullName);

    private sealed record PortfolioRequest(
        string InvestorId,
        string Name);

    private sealed record SetBaseCurrencyRequest(
        string CurrencyId);

    private sealed record CashRequest(
        string PortfolioId,
        string CurrencyId,
        int Type,
        decimal Amount,
        DateTimeOffset OccurredOn,
        string? Description);
}
