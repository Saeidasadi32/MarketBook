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
/// EN: Integration tests for request overrides and persisted portfolio risk-policy evaluation.
/// FA: تست‌های Integration ارزیابی Overrideهای Request و Policy ریسک ذخیره‌شده پرتفوی.
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

    /// <summary>
    /// EN: Verifies an active persisted policy supplies limits when the request omits them.
    /// FA: بررسی می‌کند Policy فعال ذخیره‌شده در نبود Limitهای Request، حدود را تأمین کند.
    /// </summary>
    [Fact]
    public async Task RiskLimits_Should_Fallback_To_Active_Persisted_Policy()
    {
        PortfolioScenario scenario =
            await CreateScenarioAsync();

        await CreateRiskPolicyAsync(
            scenario.PortfolioId,
            scenario.From,
            10m,
            null,
            null,
            null,
            1000m,
            null,
            null);

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

        Assert.Equal(
            "PersistedPolicy",
            root.GetProperty("limitSource").GetString());

        Assert.Equal(
            1,
            root.GetProperty("policyVersion").GetInt32());

        Assert.False(
            string.IsNullOrWhiteSpace(
                root.GetProperty("policyId").GetString()));

        Assert.Equal(
            2,
            root.GetProperty("configuredLimitCount").GetInt32());

        Assert.Equal(
            "WithinLimit",
            root.GetProperty("overallStatus").GetString());
    }

    /// <summary>
    /// EN: Verifies explicit request limits override the corresponding persisted values.
    /// FA: بررسی می‌کند Limit صریح Request روی مقدار متناظر ذخیره‌شده اولویت داشته باشد.
    /// </summary>
    [Fact]
    public async Task RiskLimits_Should_Override_Persisted_Limit_With_Request_Value()
    {
        PortfolioScenario scenario =
            await CreateScenarioAsync();

        await CreateRiskPolicyAsync(
            scenario.PortfolioId,
            scenario.From,
            null,
            null,
            null,
            null,
            1000m,
            null,
            null);

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
            "RequestOverride",
            root.GetProperty("limitSource").GetString());

        JsonElement rule =
            root.GetProperty("rules")
                .EnumerateArray()
                .Single(
                    item =>
                        item.GetProperty("code").GetString() ==
                        "MaximumDrawdownAmountBase");

        Assert.Equal(
            50m,
            rule.GetProperty("limit").GetDecimal());

        Assert.Equal(
            "Breached",
            rule.GetProperty("status").GetString());
    }

    /// <summary>
    /// EN: Verifies request overrides can be combined with different persisted fallback limits.
    /// FA: بررسی می‌کند Overrideهای Request با Limitهای متفاوت Policy ذخیره‌شده ترکیب شوند.
    /// </summary>
    [Fact]
    public async Task RiskLimits_Should_Report_Mixed_Source_When_Fallback_And_Override_Are_Both_Used()
    {
        PortfolioScenario scenario =
            await CreateScenarioAsync();

        await CreateRiskPolicyAsync(
            scenario.PortfolioId,
            scenario.From,
            10m,
            null,
            null,
            null,
            1000m,
            null,
            null);

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
            "Mixed",
            root.GetProperty("limitSource").GetString());

        Assert.Equal(
            2,
            root.GetProperty("configuredLimitCount").GetInt32());

        JsonElement volatility =
            root.GetProperty("rules")
                .EnumerateArray()
                .Single(
                    item =>
                        item.GetProperty("code").GetString() ==
                        "AnnualizedVolatility");

        Assert.Equal(
            10m,
            volatility.GetProperty("limit").GetDecimal());

        JsonElement drawdown =
            root.GetProperty("rules")
                .EnumerateArray()
                .Single(
                    item =>
                        item.GetProperty("code").GetString() ==
                        "MaximumDrawdownAmountBase");

        Assert.Equal(
            50m,
            drawdown.GetProperty("limit").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies an archived policy remains selectable for a historical To instant inside its effective interval.
    /// FA: بررسی می‌کند Policy بایگانی‌شده برای To تاریخی داخل بازه اعتبار خودش قابل انتخاب باقی بماند.
    /// </summary>
    [Fact]
    public async Task RiskLimits_Should_Select_Historical_Policy_AsOf_To()
    {
        PortfolioScenario scenario =
            await CreateScenarioAsync();

        DateTimeOffset version2EffectiveFrom =
            scenario.From.AddDays(2);

        await CreateRiskPolicyAsync(
            scenario.PortfolioId,
            scenario.From,
            null,
            null,
            null,
            null,
            1000m,
            null,
            null);

        await CreateRiskPolicyVersionAsync(
            scenario.PortfolioId,
            version2EffectiveFrom,
            null,
            null,
            null,
            null,
            50m,
            null,
            null);

        await ActivateRiskPolicyAsync(
            scenario.PortfolioId,
            2,
            version2EffectiveFrom);

        DateTimeOffset historicalTo =
            version2EffectiveFrom.AddTicks(-1);

        using HttpResponseMessage response =
            await GetRiskLimitsAsync(
                scenario.PortfolioId,
                scenario.From,
                historicalTo,
                string.Empty);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root =
            document.RootElement;

        Assert.Equal(
            "PersistedPolicy",
            root.GetProperty("limitSource").GetString());

        Assert.Equal(
            1,
            root.GetProperty("policyVersion").GetInt32());

        Assert.Equal(
            historicalTo,
            root.GetProperty("policyAsOf").GetDateTimeOffset());

        JsonElement drawdown =
            root.GetProperty("rules")
                .EnumerateArray()
                .Single(
                    item =>
                        item.GetProperty("code").GetString() ==
                        "MaximumDrawdownAmountBase");

        Assert.Equal(
            1000m,
            drawdown.GetProperty("limit").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies the newly activated version wins exactly at its EffectiveFrom boundary.
    /// FA: بررسی می‌کند نسخه تازه فعال‌شده دقیقاً در مرز EffectiveFrom انتخاب شود.
    /// </summary>
    [Fact]
    public async Task RiskLimits_Should_Select_New_Policy_At_Activation_Boundary()
    {
        PortfolioScenario scenario =
            await CreateScenarioAsync();

        DateTimeOffset version2EffectiveFrom =
            scenario.From.AddDays(2);

        await CreateRiskPolicyAsync(
            scenario.PortfolioId,
            scenario.From,
            null,
            null,
            null,
            null,
            1000m,
            null,
            null);

        await CreateRiskPolicyVersionAsync(
            scenario.PortfolioId,
            version2EffectiveFrom,
            null,
            null,
            null,
            null,
            50m,
            null,
            null);

        await ActivateRiskPolicyAsync(
            scenario.PortfolioId,
            2,
            version2EffectiveFrom);

        using HttpResponseMessage response =
            await GetRiskLimitsAsync(
                scenario.PortfolioId,
                scenario.From,
                version2EffectiveFrom,
                string.Empty);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root =
            document.RootElement;

        Assert.Equal(
            2,
            root.GetProperty("policyVersion").GetInt32());

        Assert.Equal(
            version2EffectiveFrom,
            root.GetProperty("policyAsOf").GetDateTimeOffset());

        JsonElement drawdown =
            root.GetProperty("rules")
                .EnumerateArray()
                .Single(
                    item =>
                        item.GetProperty("code").GetString() ==
                        "MaximumDrawdownAmountBase");

        Assert.Equal(
            50m,
            drawdown.GetProperty("limit").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies a future active policy is not back-applied to an earlier historical evaluation.
    /// FA: بررسی می‌کند Policy فعال با شروع اعتبار آینده روی ارزیابی تاریخی قبل از شروع اعتبار اعمال نشود.
    /// </summary>
    [Fact]
    public async Task RiskLimits_Should_Not_BackApply_Future_Policy()
    {
        PortfolioScenario scenario =
            await CreateScenarioAsync();

        DateTimeOffset policyEffectiveFrom =
            scenario.From.AddDays(2);

        await CreateRiskPolicyAsync(
            scenario.PortfolioId,
            policyEffectiveFrom,
            null,
            null,
            null,
            null,
            1000m,
            null,
            null);

        DateTimeOffset historicalTo =
            policyEffectiveFrom.AddTicks(-1);

        using HttpResponseMessage response =
            await GetRiskLimitsAsync(
                scenario.PortfolioId,
                scenario.From,
                historicalTo,
                string.Empty);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root =
            document.RootElement;

        Assert.Equal(
            "None",
            root.GetProperty("limitSource").GetString());

        Assert.Equal(
            JsonValueKind.Null,
            root.GetProperty("policyId").ValueKind);

        Assert.Equal(
            JsonValueKind.Null,
            root.GetProperty("policyVersion").ValueKind);

        Assert.Equal(
            0,
            root.GetProperty("configuredLimitCount").GetInt32());
    }

    /// <summary>
    /// EN: Verifies DOC-0045 behavior remains unchanged when neither request nor persisted limits exist.
    /// FA: بررسی می‌کند در نبود Limitهای Request و Policy ذخیره‌شده، رفتار DOC-0045 بدون تغییر بماند.
    /// </summary>
    [Fact]
    public async Task RiskLimits_Should_Preserve_NoLimitsConfigured_When_No_Active_Policy_Exists()
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

        Assert.Equal(
            "None",
            root.GetProperty("limitSource").GetString());

        Assert.Equal(
            JsonValueKind.Null,
            root.GetProperty("policyId").ValueKind);

        Assert.Equal(
            JsonValueKind.Null,
            root.GetProperty("policyVersion").ValueKind);

        Assert.Equal(
            "NoLimitsConfigured",
            root.GetProperty("overallStatus").GetString());
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

    private async Task CreateRiskPolicyAsync(
        string portfolioId,
        DateTimeOffset effectiveFrom,
        decimal? maxAnnualizedVolatility,
        decimal? maxValueAtRiskReturn,
        decimal? maxValueAtRiskAmountBase,
        decimal? maxDrawdownLossRatio,
        decimal? maxDrawdownAmountBase,
        decimal? minSharpeRatio,
        decimal? minSortinoRatio)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/risk-policy",
                new RiskPolicyRequest(
                    effectiveFrom,
                    maxAnnualizedVolatility,
                    maxValueAtRiskReturn,
                    maxValueAtRiskAmountBase,
                    maxDrawdownLossRatio,
                    maxDrawdownAmountBase,
                    minSharpeRatio,
                    minSortinoRatio));

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    private async Task CreateRiskPolicyVersionAsync(
        string portfolioId,
        DateTimeOffset effectiveFrom,
        decimal? maxAnnualizedVolatility,
        decimal? maxValueAtRiskReturn,
        decimal? maxValueAtRiskAmountBase,
        decimal? maxDrawdownLossRatio,
        decimal? maxDrawdownAmountBase,
        decimal? minSharpeRatio,
        decimal? minSortinoRatio)
    {
        using HttpResponseMessage response =
            await _client.PutAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/risk-policy",
                new RiskPolicyRequest(
                    effectiveFrom,
                    maxAnnualizedVolatility,
                    maxValueAtRiskReturn,
                    maxValueAtRiskAmountBase,
                    maxDrawdownLossRatio,
                    maxDrawdownAmountBase,
                    minSharpeRatio,
                    minSortinoRatio));

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    private async Task ActivateRiskPolicyAsync(
        string portfolioId,
        int policyVersion,
        DateTimeOffset effectiveFrom)
    {
        using HttpResponseMessage response =
            await _client.PostAsync(
                $"/api/v1/portfolios/{portfolioId}/risk-policy/{policyVersion}/activate" +
                $"?effectiveFrom={Uri.EscapeDataString(effectiveFrom.ToString("O"))}",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
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
