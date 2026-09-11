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
/// EN: Integration tests for rolling base-currency historical VaR/CVaR amounts.
/// FA: تست‌های Integration مربوط به مبالغ VaR/CVaR تاریخی Rolling در ارز پایه.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioRollingValueAtRiskAmountEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes rolling historical VaR/CVaR amount endpoint tests.
    /// FA: تست‌های Endpoint مربوط به VaR/CVaR مبلغی تاریخی Rolling را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration fixture. FA: Fixture تست Integration.</param>
    public PortfolioRollingValueAtRiskAmountEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies each rolling risk ratio uses the historical NAV at that window's own end.
    /// FA: بررسی می‌کند نسبت ریسک هر پنجره از NAV تاریخی پایان همان پنجره استفاده کند.
    /// </summary>
    [Fact]
    public async Task RollingValueAtRiskAmount_Should_Use_Each_Window_End_Nav()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "HUF",
                $"Rolling Amount HUF {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Rolling Amount Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Rolling Amount {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

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
                from.AddDays(1),
                "Dividend1"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                5,
                110m,
                from.AddDays(2),
                "Fee"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                7,
                99m,
                to,
                "Dividend2"));

        using HttpResponseMessage response =
            await GetRollingValueAtRiskAmountAsync(
                portfolioId,
                from,
                to,
                "Daily",
                2,
                0.95m);

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
            root.GetProperty("pointCount").GetInt32());

        Assert.Equal(
            2,
            root.GetProperty("calculablePointCount").GetInt32());

        JsonElement.ArrayEnumerator points =
            root.GetProperty("points").EnumerateArray();

        Assert.True(points.MoveNext());

        JsonElement first =
            points.Current;

        Assert.True(
            first.GetProperty("isComplete").GetBoolean());

        Assert.True(
            first.GetProperty("isCalculable").GetBoolean());

        Assert.Equal(
            990m,
            first.GetProperty("netAssetValueBase").GetDecimal());

        Assert.Equal(
            0.10m,
            first.GetProperty("valueAtRiskReturn").GetDecimal());

        Assert.Equal(
            99m,
            first.GetProperty("valueAtRiskAmountBase").GetDecimal());

        Assert.Equal(
            99m,
            first.GetProperty("conditionalValueAtRiskAmountBase").GetDecimal());

        Assert.True(points.MoveNext());

        JsonElement second =
            points.Current;

        Assert.True(
            second.GetProperty("isComplete").GetBoolean());

        Assert.True(
            second.GetProperty("isCalculable").GetBoolean());

        Assert.Equal(
            1089m,
            second.GetProperty("netAssetValueBase").GetDecimal());

        Assert.Equal(
            0.10m,
            second.GetProperty("valueAtRiskReturn").GetDecimal());

        Assert.Equal(
            108.9m,
            second.GetProperty("valueAtRiskAmountBase").GetDecimal());

        Assert.Equal(
            108.9m,
            second.GetProperty("conditionalValueAtRiskAmountBase").GetDecimal());

        Assert.False(points.MoveNext());
    }

    /// <summary>
    /// EN: Verifies no monetary points are fabricated before a full rolling window exists.
    /// FA: بررسی می‌کند پیش از تشکیل پنجره کامل Rolling هیچ نقطه مبلغی ساخته نشود.
    /// </summary>
    [Fact]
    public async Task RollingValueAtRiskAmount_Should_Return_No_Points_When_Window_Is_Not_Full()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "RON",
                $"Rolling Amount RON {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Rolling Amount Short Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Rolling Amount Short {Guid.NewGuid():N}");

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
                100m,
                from.AddDays(1),
                "Dividend"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                5,
                110m,
                to,
                "Fee"));

        using HttpResponseMessage response =
            await GetRollingValueAtRiskAmountAsync(
                portfolioId,
                from,
                to,
                "Daily",
                3,
                0.95m);

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
            0,
            root.GetProperty("pointCount").GetInt32());

        Assert.Equal(
            0,
            root.GetProperty("calculablePointCount").GetInt32());

        Assert.Equal(
            0,
            root.GetProperty("points").GetArrayLength());
    }

    /// <summary>
    /// EN: Verifies invalid rolling-window input is rejected by the composed DOC-0038 contract.
    /// FA: بررسی می‌کند اندازه نامعتبر پنجره توسط قرارداد ترکیبی DOC-0038 رد شود.
    /// </summary>
    [Fact]
    public async Task RollingValueAtRiskAmount_Should_Reject_Invalid_Window()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "BGN",
                $"Rolling Amount BGN {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Rolling Amount Invalid Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Rolling Amount Invalid {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        DateTimeOffset to =
            from.AddDays(2);

        await SetBaseCurrencyAsync(
            portfolioId,
            currencyId);

        using HttpResponseMessage response =
            await GetRollingValueAtRiskAmountAsync(
                portfolioId,
                from,
                to,
                "Daily",
                1,
                0.95m);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private async Task<HttpResponseMessage> GetRollingValueAtRiskAmountAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval,
        int windowPeriods,
        decimal confidenceLevel)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/risk/var/rolling/amount" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}" +
            $"&windowPeriods={windowPeriods}" +
            $"&confidenceLevel={confidenceLevel.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

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
