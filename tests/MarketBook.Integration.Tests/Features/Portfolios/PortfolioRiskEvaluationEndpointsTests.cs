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
/// Integration tests for persisted portfolio risk-evaluation snapshots.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioRiskEvaluationEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>Initializes persisted risk-evaluation endpoint tests.</summary>
    public PortfolioRiskEvaluationEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>Verifies POST persists the complete evaluation snapshot and GET by id returns it.</summary>
    [Fact]
    public async Task Create_Should_Persist_And_Detail_Should_Return_Snapshot()
    {
        PortfolioScenario scenario = await CreateScenarioAsync();

        using HttpResponseMessage create =
            await CreateEvaluationAsync(
                scenario,
                maxAnnualizedVolatility: 10m,
                maxDrawdownAmountBase: 50m);

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        using JsonDocument created = await ReadJsonAsync(create);
        JsonElement root = created.RootElement;

        string evaluationId = root.GetProperty("id").GetString()!;

        Assert.Equal(scenario.PortfolioId, root.GetProperty("portfolioId").GetString());
        Assert.Equal(scenario.From, root.GetProperty("from").GetDateTimeOffset());
        Assert.Equal(scenario.To, root.GetProperty("to").GetDateTimeOffset());
        Assert.Equal("Daily", root.GetProperty("interval").GetString());
        Assert.Equal(0.95m, root.GetProperty("confidenceLevel").GetDecimal());
        Assert.Equal(0.01m, root.GetProperty("riskFreeRateAnnual").GetDecimal());
        Assert.Equal(0.02m, root.GetProperty("minimumAcceptableReturnAnnual").GetDecimal());
        Assert.Equal("RequestOverride", root.GetProperty("limitSource").GetString());
        Assert.Equal(2, root.GetProperty("configuredLimitCount").GetInt32());
        Assert.Equal(7, root.GetProperty("rules").GetArrayLength());

        JsonElement drawdown =
            root.GetProperty("rules")
                .EnumerateArray()
                .Single(item =>
                    item.GetProperty("code").GetString() ==
                    "MaximumDrawdownAmountBase");

        Assert.Equal(50m, drawdown.GetProperty("limit").GetDecimal());
        Assert.Equal(150m, drawdown.GetProperty("actual").GetDecimal());
        Assert.Equal("Breached", drawdown.GetProperty("status").GetString());

        using HttpResponseMessage detail =
            await _client.GetAsync(
                $"/api/v1/portfolios/{scenario.PortfolioId}/risk-evaluations/{evaluationId}");

        Assert.Equal(HttpStatusCode.OK, detail.StatusCode);

        using JsonDocument persisted = await ReadJsonAsync(detail);
        Assert.Equal(
            evaluationId,
            persisted.RootElement.GetProperty("id").GetString());
        Assert.Equal(
            root.GetProperty("evaluatedOn").GetDateTimeOffset(),
            persisted.RootElement.GetProperty("evaluatedOn").GetDateTimeOffset());
    }

    /// <summary>Verifies history returns persisted evaluations newest first.</summary>
    [Fact]
    public async Task History_Should_Return_Newest_Evaluation_First()
    {
        PortfolioScenario scenario = await CreateScenarioAsync();

        using HttpResponseMessage first =
            await CreateEvaluationAsync(
                scenario,
                maxAnnualizedVolatility: 10m);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        using JsonDocument firstJson = await ReadJsonAsync(first);
        string firstId = firstJson.RootElement.GetProperty("id").GetString()!;

        await Task.Delay(20);

        using HttpResponseMessage second =
            await CreateEvaluationAsync(
                scenario,
                maxDrawdownAmountBase: 50m);

        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        using JsonDocument secondJson = await ReadJsonAsync(second);
        string secondId = secondJson.RootElement.GetProperty("id").GetString()!;

        using HttpResponseMessage history =
            await _client.GetAsync(
                $"/api/v1/portfolios/{scenario.PortfolioId}/risk-evaluations");

        Assert.Equal(HttpStatusCode.OK, history.StatusCode);

        using JsonDocument historyJson = await ReadJsonAsync(history);
        JsonElement[] items = historyJson.RootElement.EnumerateArray().ToArray();

        Assert.Equal(2, items.Length);
        Assert.Equal(secondId, items[0].GetProperty("id").GetString());
        Assert.Equal(firstId, items[1].GetProperty("id").GetString());
    }

    /// <summary>Verifies detail lookup is scoped to the portfolio route.</summary>
    [Fact]
    public async Task Detail_Should_Not_Return_Evaluation_From_Another_Portfolio()
    {
        PortfolioScenario source = await CreateScenarioAsync();

        using HttpResponseMessage create =
            await CreateEvaluationAsync(
                source,
                maxDrawdownAmountBase: 50m);

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        using JsonDocument created = await ReadJsonAsync(create);
        string evaluationId = created.RootElement.GetProperty("id").GetString()!;

        string otherPortfolioId = await CreateEmptyPortfolioAsync();

        using HttpResponseMessage detail =
            await _client.GetAsync(
                $"/api/v1/portfolios/{otherPortfolioId}/risk-evaluations/{evaluationId}");

        Assert.Equal(HttpStatusCode.NotFound, detail.StatusCode);
    }

    /// <summary>Verifies a persisted snapshot freezes historical policy identity and version.</summary>
    [Fact]
    public async Task Create_Should_Freeze_Historical_Effective_Policy_Metadata()
    {
        PortfolioScenario scenario = await CreateScenarioAsync();

        DateTimeOffset version2EffectiveFrom = scenario.From.AddDays(2);

        await CreateRiskPolicyAsync(
            scenario.PortfolioId,
            scenario.From,
            maxDrawdownAmountBase: 1000m);

        await CreateRiskPolicyVersionAsync(
            scenario.PortfolioId,
            version2EffectiveFrom,
            maxDrawdownAmountBase: 50m);

        await ActivateRiskPolicyAsync(
            scenario.PortfolioId,
            2,
            version2EffectiveFrom);

        PortfolioScenario historical =
            scenario with
            {
                To = version2EffectiveFrom.AddTicks(-1)
            };

        using HttpResponseMessage create =
            await CreateEvaluationAsync(historical);

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        using JsonDocument document = await ReadJsonAsync(create);
        JsonElement root = document.RootElement;

        Assert.Equal("PersistedPolicy", root.GetProperty("limitSource").GetString());
        Assert.Equal(1, root.GetProperty("policyVersion").GetInt32());
        Assert.Equal(historical.To, root.GetProperty("policyAsOf").GetDateTimeOffset());
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("policyId").GetString()));

        JsonElement drawdown =
            root.GetProperty("rules")
                .EnumerateArray()
                .Single(item =>
                    item.GetProperty("code").GetString() ==
                    "MaximumDrawdownAmountBase");

        Assert.Equal(1000m, drawdown.GetProperty("limit").GetDecimal());
    }

    /// <summary>Verifies persisted-policy fallback and request override are both frozen in a mixed snapshot.</summary>
    [Fact]
    public async Task Create_Should_Persist_Mixed_Policy_And_Request_Overrides()
    {
        PortfolioScenario scenario = await CreateScenarioAsync();

        await CreateRiskPolicyAsync(
            scenario.PortfolioId,
            scenario.From,
            maxAnnualizedVolatility: 10m,
            maxDrawdownAmountBase: 1000m);

        using HttpResponseMessage create =
            await CreateEvaluationAsync(
                scenario,
                maxDrawdownAmountBase: 50m);

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        using JsonDocument document = await ReadJsonAsync(create);
        JsonElement root = document.RootElement;

        Assert.Equal("Mixed", root.GetProperty("limitSource").GetString());
        Assert.Equal(1, root.GetProperty("policyVersion").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("policyId").GetString()));

        JsonElement volatility =
            root.GetProperty("rules")
                .EnumerateArray()
                .Single(item =>
                    item.GetProperty("code").GetString() ==
                    "AnnualizedVolatility");

        JsonElement drawdown =
            root.GetProperty("rules")
                .EnumerateArray()
                .Single(item =>
                    item.GetProperty("code").GetString() ==
                    "MaximumDrawdownAmountBase");

        Assert.Equal(10m, volatility.GetProperty("limit").GetDecimal());
        Assert.Equal(50m, drawdown.GetProperty("limit").GetDecimal());
        Assert.Equal("Breached", drawdown.GetProperty("status").GetString());
    }

    /// <summary>Verifies the legacy GET evaluation endpoint remains side-effect free.</summary>
    [Fact]
    public async Task ReadOnly_Evaluate_Should_Not_Persist_Risk_Evaluation()
    {
        PortfolioScenario scenario = await CreateScenarioAsync();

        using HttpResponseMessage before =
            await _client.GetAsync(
                $"/api/v1/portfolios/{scenario.PortfolioId}/risk-evaluations");

        Assert.Equal(HttpStatusCode.OK, before.StatusCode);
        using JsonDocument beforeJson = await ReadJsonAsync(before);
        Assert.Empty(beforeJson.RootElement.EnumerateArray());

        using HttpResponseMessage evaluate =
            await _client.GetAsync(
                $"/api/v1/portfolios/{scenario.PortfolioId}/performance/risk/limits/evaluate" +
                $"?from={Uri.EscapeDataString(scenario.From.ToString("O"))}" +
                $"&to={Uri.EscapeDataString(scenario.To.ToString("O"))}" +
                "&interval=Daily" +
                "&confidenceLevel=0.95" +
                "&riskFreeRateAnnual=0" +
                "&minimumAcceptableReturnAnnual=0" +
                "&maxDrawdownAmountBase=50");

        Assert.Equal(HttpStatusCode.OK, evaluate.StatusCode);

        using HttpResponseMessage after =
            await _client.GetAsync(
                $"/api/v1/portfolios/{scenario.PortfolioId}/risk-evaluations");

        Assert.Equal(HttpStatusCode.OK, after.StatusCode);
        using JsonDocument afterJson = await ReadJsonAsync(after);
        Assert.Empty(afterJson.RootElement.EnumerateArray());
    }

    private async Task<PortfolioScenario> CreateScenarioAsync()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "CHF",
                $"Risk Evaluation CHF {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Risk Evaluation Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Risk Evaluation {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        DateTimeOffset day1 = from.AddDays(1);
        DateTimeOffset day2 = from.AddDays(2);
        DateTimeOffset to = from.AddDays(3);

        await SetBaseCurrencyAsync(portfolioId, currencyId);

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

        return new PortfolioScenario(portfolioId, from, to);
    }

    private async Task<HttpResponseMessage> CreateEvaluationAsync(
        PortfolioScenario scenario,
        decimal? maxAnnualizedVolatility = null,
        decimal? maxValueAtRiskReturn = null,
        decimal? maxValueAtRiskAmountBase = null,
        decimal? maxDrawdownLossRatio = null,
        decimal? maxDrawdownAmountBase = null,
        decimal? minSharpeRatio = null,
        decimal? minSortinoRatio = null)
        => await _client.PostAsJsonAsync(
            $"/api/v1/portfolios/{scenario.PortfolioId}/risk-evaluations",
            new RiskEvaluationRequest(
                scenario.From,
                scenario.To,
                "Daily",
                0.95m,
                0.01m,
                0.02m,
                maxAnnualizedVolatility,
                maxValueAtRiskReturn,
                maxValueAtRiskAmountBase,
                maxDrawdownLossRatio,
                maxDrawdownAmountBase,
                minSharpeRatio,
                minSortinoRatio));

    private async Task<string> CreateEmptyPortfolioAsync()
    {
        string investorId =
            await CreateInvestorAsync(
                $"Other Risk Evaluation Owner {Guid.NewGuid():N}");

        return await CreatePortfolioAsync(
            investorId,
            $"Other Risk Evaluation {Guid.NewGuid():N}");
    }

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

    private async Task CreateRiskPolicyAsync(
        string portfolioId,
        DateTimeOffset effectiveFrom,
        decimal? maxAnnualizedVolatility = null,
        decimal? maxDrawdownAmountBase = null)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/risk-policy",
                new RiskPolicyRequest(
                    effectiveFrom,
                    maxAnnualizedVolatility,
                    null,
                    null,
                    null,
                    maxDrawdownAmountBase,
                    null,
                    null));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task CreateRiskPolicyVersionAsync(
        string portfolioId,
        DateTimeOffset effectiveFrom,
        decimal? maxAnnualizedVolatility = null,
        decimal? maxDrawdownAmountBase = null)
    {
        using HttpResponseMessage response =
            await _client.PutAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/risk-policy",
                new RiskPolicyRequest(
                    effectiveFrom,
                    maxAnnualizedVolatility,
                    null,
                    null,
                    null,
                    maxDrawdownAmountBase,
                    null,
                    null));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task ActivateRiskPolicyAsync(
        string portfolioId,
        int version,
        DateTimeOffset effectiveFrom)
    {
        using HttpResponseMessage response =
            await _client.PostAsync(
                $"/api/v1/portfolios/{portfolioId}/risk-policy/{version}/activate" +
                $"?effectiveFrom={Uri.EscapeDataString(effectiveFrom.ToString("O"))}",
                null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record PortfolioScenario(
        string PortfolioId,
        DateTimeOffset From,
        DateTimeOffset To);

    private sealed record RiskEvaluationRequest(
        DateTimeOffset From,
        DateTimeOffset To,
        string Interval,
        decimal ConfidenceLevel,
        decimal RiskFreeRateAnnual,
        decimal MinimumAcceptableReturnAnnual,
        decimal? MaxAnnualizedVolatility,
        decimal? MaxValueAtRiskReturn,
        decimal? MaxValueAtRiskAmountBase,
        decimal? MaxDrawdownLossRatio,
        decimal? MaxDrawdownAmountBase,
        decimal? MinSharpeRatio,
        decimal? MinSortinoRatio);

    private sealed record CurrencyRequest(
        string Code,
        string Name,
        int DecimalPlaces);

    private sealed record InvestorRequest(string FullName);

    private sealed record PortfolioRequest(
        string InvestorId,
        string Name);

    private sealed record SetBaseCurrencyRequest(string CurrencyId);

    private sealed record RiskPolicyRequest(
        DateTimeOffset EffectiveFrom,
        decimal? MaxAnnualizedVolatility,
        decimal? MaxValueAtRiskReturn,
        decimal? MaxValueAtRiskAmountBase,
        decimal? MaxDrawdownLossRatio,
        decimal? MaxDrawdownAmountBase,
        decimal? MinSharpeRatio,
        decimal? MinSortinoRatio);

    private sealed record CashRequest(
        string PortfolioId,
        string CurrencyId,
        int Type,
        decimal Amount,
        DateTimeOffset OccurredOn,
        string? Description);
}
