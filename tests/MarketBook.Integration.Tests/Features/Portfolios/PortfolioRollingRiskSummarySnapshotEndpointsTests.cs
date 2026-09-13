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
/// EN: Integration tests for the latest rolling risk-summary snapshot.
/// FA: تست‌های Integration مربوط به آخرین snapshot خلاصه ریسک Rolling.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioRollingRiskSummarySnapshotEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes rolling risk-summary snapshot endpoint tests.
    /// FA: تست‌های Endpoint مربوط به snapshot خلاصه ریسک Rolling را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioRollingRiskSummarySnapshotEndpointsTests(
        IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies the endpoint exposes only the latest full-window point from both rolling sources.
    /// FA: بررسی می‌کند Endpoint فقط آخرین نقطه پنجره کامل را از هر دو منبع Rolling ارائه دهد.
    /// </summary>
    [Fact]
    public async Task RollingRiskSummary_Should_Return_Latest_Points()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "DKK",
                $"Rolling Summary DKK {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Rolling Summary Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Rolling Summary {Guid.NewGuid():N}");

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
                110m,
                day2,
                "Fee"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                7,
                99m,
                to,
                "Dividend"));

        using HttpResponseMessage response =
            await GetRollingSummaryAsync(
                portfolioId,
                from,
                to,
                "Daily",
                2,
                0.95m,
                0m,
                0m);

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
            2,
            root.GetProperty("windowPeriods").GetInt32());

        JsonElement rollingRisk =
            root.GetProperty("rollingRisk");

        Assert.True(
            rollingRisk.GetProperty("isComplete").GetBoolean());

        Assert.True(
            rollingRisk.GetProperty("hasPoint").GetBoolean());

        Assert.Equal(
            2,
            rollingRisk.GetProperty("pointCount").GetInt32());

        Assert.Equal(
            2,
            rollingRisk.GetProperty("observationCount").GetInt32());

        Assert.Equal(
            to,
            rollingRisk.GetProperty("windowTo").GetDateTimeOffset());

        JsonElement rollingValueAtRisk =
            root.GetProperty("rollingValueAtRisk");

        Assert.True(
            rollingValueAtRisk.GetProperty("isComplete").GetBoolean());

        Assert.True(
            rollingValueAtRisk.GetProperty("hasPoint").GetBoolean());

        Assert.Equal(
            2,
            rollingValueAtRisk.GetProperty("pointCount").GetInt32());

        Assert.True(
            rollingValueAtRisk.GetProperty("isCalculable").GetBoolean());

        Assert.Equal(
            to,
            rollingValueAtRisk.GetProperty("windowTo").GetDateTimeOffset());

        Assert.Equal(
            1089m,
            rollingValueAtRisk.GetProperty("netAssetValueBase").GetDecimal());

        Assert.Equal(
            108.9m,
            rollingValueAtRisk.GetProperty("valueAtRiskAmountBase").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies complete sources with insufficient observations produce no fabricated latest point.
    /// FA: بررسی می‌کند منبع کامل با مشاهدات ناکافی هیچ نقطه آخر ساختگی تولید نکند.
    /// </summary>
    [Fact]
    public async Task RollingRiskSummary_Should_Return_No_Point_Before_Full_Window()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "NOK",
                $"Rolling Summary NOK {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Rolling Summary Short Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Rolling Summary Short {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        DateTimeOffset to =
            from.AddDays(2);

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
                50m,
                from.AddDays(1),
                "Dividend"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                5,
                25m,
                to,
                "Fee"));

        using HttpResponseMessage response =
            await GetRollingSummaryAsync(
                portfolioId,
                from,
                to,
                "Daily",
                3,
                0.95m,
                0m,
                0m);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root =
            document.RootElement;

        Assert.True(
            root.GetProperty("isComplete").GetBoolean());

        JsonElement rollingRisk =
            root.GetProperty("rollingRisk");

        Assert.False(
            rollingRisk.GetProperty("hasPoint").GetBoolean());

        Assert.Equal(
            0,
            rollingRisk.GetProperty("pointCount").GetInt32());

        Assert.Equal(
            JsonValueKind.Null,
            rollingRisk.GetProperty("windowTo").ValueKind);

        JsonElement rollingValueAtRisk =
            root.GetProperty("rollingValueAtRisk");

        Assert.False(
            rollingValueAtRisk.GetProperty("hasPoint").GetBoolean());

        Assert.Equal(
            0,
            rollingValueAtRisk.GetProperty("pointCount").GetInt32());

        Assert.Equal(
            JsonValueKind.Null,
            rollingValueAtRisk.GetProperty("valueAtRiskAmountBase").ValueKind);
    }

    /// <summary>
    /// EN: Verifies an invalid rolling window is rejected by the composed source contract.
    /// FA: بررسی می‌کند اندازه پنجره Rolling نامعتبر توسط قرارداد منبع ترکیبی رد شود.
    /// </summary>
    [Fact]
    public async Task RollingRiskSummary_Should_Reject_Invalid_Window()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "CHF",
                $"Rolling Summary CHF {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Rolling Summary Invalid Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Rolling Summary Invalid {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(
            portfolioId,
            currencyId);

        using HttpResponseMessage response =
            await GetRollingSummaryAsync(
                portfolioId,
                from,
                from.AddDays(2),
                "Daily",
                1,
                0.95m,
                0m,
                0m);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private async Task<HttpResponseMessage> GetRollingSummaryAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval,
        int windowPeriods,
        decimal confidenceLevel,
        decimal riskFreeRateAnnual,
        decimal minimumAcceptableReturnAnnual)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/risk/summary/rolling" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}" +
            $"&windowPeriods={windowPeriods}" +
            $"&confidenceLevel={confidenceLevel}" +
            $"&riskFreeRateAnnual={riskFreeRateAnnual}" +
            $"&minimumAcceptableReturnAnnual={minimumAcceptableReturnAnnual}");

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
