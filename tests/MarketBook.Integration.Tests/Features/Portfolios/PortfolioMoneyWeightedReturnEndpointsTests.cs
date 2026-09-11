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
/// EN: Integration tests for portfolio money-weighted return (XIRR).
/// FA: تست‌های Integration مربوط به بازده پول‌وزن پرتفوی (XIRR).
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioMoneyWeightedReturnEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes XIRR endpoint tests.
    /// FA: تست‌های Endpoint مربوط به XIRR را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioMoneyWeightedReturnEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies a one-year 10 percent gain produces approximately 10 percent XIRR.
    /// FA: بررسی می‌کند رشد ۱۰ درصدی طی یک سال تقریباً XIRR برابر ۱۰ درصد ایجاد کند.
    /// </summary>
    [Fact]
    public async Task Xirr_Should_Return_Approximately_Ten_Percent_For_One_Year_Gain()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("EUR", $"XIRR EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"XIRR Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"XIRR {Guid.NewGuid():N}");

        DateTimeOffset from = new(2025, 9, 10, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, currencyId);

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null, null, null));

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 7, 100m, to.AddDays(-1), null, null, "Dividend"));

        using HttpResponseMessage response =
            await GetXirrAsync(portfolioId, from, to);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.True(root.GetProperty("hasValidCashFlowSigns").GetBoolean());
        Assert.True(root.GetProperty("hasSolution").GetBoolean());

        decimal xirr = root.GetProperty("annualizedMoneyWeightedReturn").GetDecimal();
        Assert.InRange(xirr, 0.0999m, 0.1001m);
    }

    /// <summary>
    /// EN: Verifies investor-perspective signs for beginning NAV, deposit, withdrawal, and terminal NAV.
    /// FA: علامت جریان‌ها را از دید سرمایه‌گذار برای NAV ابتدا، واریز، برداشت و NAV نهایی بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Xirr_Should_Build_Investor_Perspective_Cash_Flow_Signs()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("GBP", $"XIRR GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"XIRR Signs Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"XIRR Signs {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 12, 31, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, currencyId);

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null, null, null));

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 1, 500m, from.AddMonths(3), null, null, null));

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 2, 200m, from.AddMonths(6), null, null, null));

        using HttpResponseMessage response =
            await GetXirrAsync(portfolioId, from, to);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] cashFlows =
            document.RootElement.GetProperty("cashFlows").EnumerateArray().ToArray();

        Assert.Contains(
            cashFlows,
            item =>
                item.GetProperty("kind").GetString() == "BeginningNAV" &&
                item.GetProperty("amountBase").GetDecimal() == -1000m);

        Assert.Contains(
            cashFlows,
            item =>
                item.GetProperty("kind").GetString() == "Deposit" &&
                item.GetProperty("amountBase").GetDecimal() == -500m);

        Assert.Contains(
            cashFlows,
            item =>
                item.GetProperty("kind").GetString() == "Withdrawal" &&
                item.GetProperty("amountBase").GetDecimal() == 200m);

        Assert.Contains(
            cashFlows,
            item =>
                item.GetProperty("kind").GetString() == "TerminalNAV" &&
                item.GetProperty("amountBase").GetDecimal() == 1300m);
    }

    /// <summary>
    /// EN: Verifies an incomplete historical FX translation prevents an XIRR result instead of treating the flow as zero.
    /// FA: بررسی می‌کند نبود FX تاریخی نتیجه XIRR را ناقص کند و جریان به‌اشتباه صفر فرض نشود.
    /// </summary>
    [Fact]
    public async Task Xirr_Should_Be_Incomplete_When_External_Flow_Fx_Is_Missing()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string usdId = await GetListingQuoteCurrencyAsync(listingId);
        string eurId = await CreateCurrencyAsync("EUR", $"XIRR Missing FX EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"XIRR Missing FX Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"XIRR Missing FX {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 12, 31, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, eurId);

        await CreateCashAsync(new CashRequest(
            portfolioId, eurId, 1, 1000m, from.AddDays(-1), null, null, null));

        await CreateCashAsync(new CashRequest(
            portfolioId, usdId, 1, 100m, from.AddMonths(3), null, null, null));

        using HttpResponseMessage response =
            await GetXirrAsync(portfolioId, from, to);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.False(root.GetProperty("isComplete").GetBoolean());
        Assert.False(root.GetProperty("hasSolution").GetBoolean());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("annualizedMoneyWeightedReturn").ValueKind);
    }

    private async Task<HttpResponseMessage> GetXirrAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/xirr" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}");

    private async Task<string> CreateCurrencyAsync(string code, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/currencies",
                new CurrencyRequest(code, name, 2));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> CreateInvestorAsync(string fullName)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/investors",
                new InvestorRequest(fullName));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> CreatePortfolioAsync(string investorId, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(investorId, name));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task SetBaseCurrencyAsync(string portfolioId, string currencyId)
    {
        using HttpResponseMessage response =
            await _client.PatchAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/base-currency",
                new SetBaseCurrencyRequest(currencyId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<string> GetListingQuoteCurrencyAsync(string listingId)
    {
        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/listings/{listingId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? currencyId = document.RootElement.GetProperty("quoteCurrencyId").GetString();

        Assert.False(string.IsNullOrWhiteSpace(currencyId));
        return currencyId!;
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
        string? ReferenceType,
        string? ReferenceId,
        string? Description);
}
