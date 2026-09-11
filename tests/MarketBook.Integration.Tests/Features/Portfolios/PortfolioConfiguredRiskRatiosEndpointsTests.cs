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
/// EN: Integration tests for configurable risk-free rate and minimum acceptable return.
/// FA: تست‌های Integration نرخ بدون‌ریسک و حداقل بازده قابل‌قبول قابل‌تنظیم.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioConfiguredRiskRatiosEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes configured risk-ratio endpoint tests.
    /// FA: تست‌های Endpoint نسبت‌های ریسک قابل‌تنظیم را مقداردهی می‌کند.
    /// </summary>
    public PortfolioConfiguredRiskRatiosEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies a positive risk-free rate lowers Sharpe by the expected arithmetic periodic amount.
    /// FA: بررسی می‌کند نرخ بدون‌ریسک مثبت، Sharpe را به میزان دوره‌ای حسابی مورد انتظار کاهش دهد.
    /// </summary>
    [Fact]
    public async Task RiskRatios_Should_Apply_Configured_Risk_Free_Rate_To_Sharpe()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("EUR", $"Configured EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Configured RF Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Configured RF {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset dayOne = from.AddDays(1);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, dayOne, "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 55m, to, "Fee"));

        decimal riskFreeRateAnnual = 0.365m;

        using HttpResponseMessage response =
            await GetRiskRatiosAsync(
                portfolioId,
                from,
                to,
                "Daily",
                riskFreeRateAnnual,
                0m);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.Equal(riskFreeRateAnnual, root.GetProperty("riskFreeRateAnnual").GetDecimal());
        Assert.Equal(0.001m, root.GetProperty("riskFreeRatePeriodic").GetDecimal());

        decimal expectedSharpe =
            ((0.025m - 0.001m) / (decimal)Math.Sqrt(0.01125d)) *
            (decimal)Math.Sqrt(365d);

        Assert.InRange(
            root.GetProperty("sharpeRatio").GetDecimal(),
            expectedSharpe - 0.0001m,
            expectedSharpe + 0.0001m);
    }

    /// <summary>
    /// EN: Verifies configured MAR recalculates the Sortino downside denominator against that target.
    /// FA: بررسی می‌کند MAR قابل‌تنظیم، مخرج انحراف نزولی Sortino را نسبت به همان هدف بازمحاسبه کند.
    /// </summary>
    [Fact]
    public async Task RiskRatios_Should_Recalculate_Sortino_Downside_Against_Configured_Mar()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("GBP", $"Configured GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Configured MAR Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Configured MAR {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset dayOne = from.AddDays(1);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, dayOne, "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 55m, to, "Fee"));

        decimal minimumAcceptableReturnAnnual = 0.365m;
        decimal targetPeriodic = 0.001m;

        using HttpResponseMessage response =
            await GetRiskRatiosAsync(
                portfolioId,
                from,
                to,
                "Daily",
                0m,
                minimumAcceptableReturnAnnual);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.Equal(
            minimumAcceptableReturnAnnual,
            root.GetProperty("minimumAcceptableReturnAnnual").GetDecimal());

        Assert.Equal(
            targetPeriodic,
            root.GetProperty("minimumAcceptableReturnPeriodic").GetDecimal());

        decimal expectedDownside =
            (decimal)Math.Sqrt(
                ((0d * 0d) + (-0.051d * -0.051d)) / 2d);

        decimal expectedSortino =
            ((0.025m - targetPeriodic) / expectedDownside) *
            (decimal)Math.Sqrt(365d);

        Assert.InRange(
            root.GetProperty("periodicDownsideDeviationRelativeToMinimumAcceptableReturn").GetDecimal(),
            expectedDownside - 0.000001m,
            expectedDownside + 0.000001m);

        Assert.InRange(
            root.GetProperty("sortinoRatio").GetDecimal(),
            expectedSortino - 0.0001m,
            expectedSortino + 0.0001m);
    }

    /// <summary>
    /// EN: Verifies omitted configured rates preserve the zero-rate DOC-0034 behavior.
    /// FA: بررسی می‌کند حذف پارامترهای نرخ، رفتار نرخ صفر DOC-0034 را حفظ کند.
    /// </summary>
    [Fact]
    public async Task RiskRatios_Should_Remain_Backward_Compatible_When_Rates_Are_Omitted()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("CHF", $"Configured CHF {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Configured Default Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Configured Default {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset dayOne = from.AddDays(1);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, dayOne, "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 55m, to, "Fee"));

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolios/{portfolioId}/performance/risk-ratios" +
                $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
                $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
                "&interval=Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.Equal(0m, root.GetProperty("riskFreeRateAnnual").GetDecimal());
        Assert.Equal(0m, root.GetProperty("riskFreeRatePeriodic").GetDecimal());
        Assert.Equal(0m, root.GetProperty("minimumAcceptableReturnAnnual").GetDecimal());
        Assert.Equal(0m, root.GetProperty("minimumAcceptableReturnPeriodic").GetDecimal());
        Assert.True(root.GetProperty("isSharpeCalculable").GetBoolean());
        Assert.True(root.GetProperty("isSortinoCalculable").GetBoolean());
    }

    private async Task<HttpResponseMessage> GetRiskRatiosAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval,
        decimal riskFreeRateAnnual,
        decimal minimumAcceptableReturnAnnual)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/risk-ratios" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}" +
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
