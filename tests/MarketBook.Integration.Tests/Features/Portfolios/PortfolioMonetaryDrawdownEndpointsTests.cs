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
/// EN: Integration tests for monetary portfolio drawdown analytics.
/// FA: تست‌های Integration مربوط به تحلیل افت سرمایه مبلغی پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioMonetaryDrawdownEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes monetary drawdown endpoint tests.
    /// FA: تست‌های Endpoint افت سرمایه مبلغی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioMonetaryDrawdownEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies a 10% TWR drawdown from equivalent 1100 exposure to 990 produces a 110 base-currency loss.
    /// FA: بررسی می‌کند افت TWR ده‌درصدی از exposure معادل ۱۱۰۰ به ۹۹۰، زیان ۱۱۰ واحد ارز پایه تولید کند.
    /// </summary>
    [Fact]
    public async Task MonetaryDrawdown_Should_Translate_Drawdown_Using_Equivalent_Peak_Exposure()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "CZK",
                $"Monetary Drawdown CZK {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Monetary Drawdown Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Monetary Drawdown {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        DateTimeOffset trough =
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
                trough,
                "Fee"));

        using HttpResponseMessage response =
            await GetMonetaryDrawdownAsync(
                portfolioId,
                from,
                trough,
                "Daily");

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
            -0.10m,
            root.GetProperty("currentDrawdown").GetDecimal());

        Assert.Equal(
            110m,
            root.GetProperty("currentDrawdownAmountBase").GetDecimal());

        Assert.Equal(
            -0.10m,
            root.GetProperty("maximumDrawdown").GetDecimal());

        Assert.Equal(
            110m,
            root.GetProperty("maximumDrawdownAmountBase").GetDecimal());

        JsonElement[] points =
            root.GetProperty("points")
                .EnumerateArray()
                .ToArray();

        Assert.Equal(
            3,
            points.Length);

        JsonElement troughPoint =
            points[2];

        Assert.True(
            troughPoint.GetProperty("isComplete").GetBoolean());

        Assert.True(
            troughPoint.GetProperty("isCalculable").GetBoolean());

        Assert.Equal(
            990m,
            troughPoint.GetProperty("netAssetValueBase").GetDecimal());

        Assert.Equal(
            1100m,
            troughPoint.GetProperty("equivalentPeakNetAssetValueBase").GetDecimal());

        Assert.Equal(
            110m,
            troughPoint.GetProperty("drawdownAmountBase").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies external capital flows alone do not create monetary drawdown.
    /// FA: بررسی می‌کند جریان سرمایه خارجی به‌تنهایی افت مبلغی ساختگی ایجاد نکند.
    /// </summary>
    [Fact]
    public async Task MonetaryDrawdown_Should_Remain_Zero_For_External_Flows_Only()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "PLN",
                $"Monetary Drawdown PLN {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Monetary Drawdown Flow Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Monetary Drawdown Flow {Guid.NewGuid():N}");

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
                2,
                200m,
                from.AddDays(1),
                "Withdrawal"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                1,
                400m,
                to,
                "Deposit"));

        using HttpResponseMessage response =
            await GetMonetaryDrawdownAsync(
                portfolioId,
                from,
                to,
                "Daily");

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
            0m,
            root.GetProperty("currentDrawdown").GetDecimal());

        Assert.Equal(
            0m,
            root.GetProperty("currentDrawdownAmountBase").GetDecimal());

        Assert.Equal(
            0m,
            root.GetProperty("maximumDrawdown").GetDecimal());

        Assert.Equal(
            0m,
            root.GetProperty("maximumDrawdownAmountBase").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies an invalid interval is rejected through the composed DOC-0031 contract.
    /// FA: بررسی می‌کند فاصله نمونه‌برداری نامعتبر از قرارداد ترکیبی DOC-0031 رد شود.
    /// </summary>
    [Fact]
    public async Task MonetaryDrawdown_Should_Reject_Invalid_Interval()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "ISK",
                $"Monetary Drawdown ISK {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Monetary Drawdown Invalid Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Monetary Drawdown Invalid {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        DateTimeOffset to =
            from.AddDays(2);

        await SetBaseCurrencyAsync(
            portfolioId,
            currencyId);

        using HttpResponseMessage response =
            await GetMonetaryDrawdownAsync(
                portfolioId,
                from,
                to,
                "Monthly");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private async Task<HttpResponseMessage> GetMonetaryDrawdownAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/drawdown/amount" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}");

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
