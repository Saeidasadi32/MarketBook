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
/// EN: Integration tests for combined TWR/XIRR performance comparison.
/// FA: تست‌های Integration مربوط به مقایسه ترکیبی عملکرد TWR/XIRR.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioPerformanceComparisonEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes performance comparison endpoint tests.
    /// FA: تست‌های Endpoint مقایسه عملکرد را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioPerformanceComparisonEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies a one-year return without external flows exposes matching TWR and XIRR.
    /// FA: بررسی می‌کند بازده یک‌ساله بدون جریان خارجی، TWR و XIRR متناظر را برگرداند.
    /// </summary>
    [Fact]
    public async Task Comparison_Should_Return_Both_Returns_When_Data_Is_Complete()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("EUR", $"Comparison EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Comparison Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Comparison {Guid.NewGuid():N}");

        DateTimeOffset from = new(2025, 9, 10, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, currencyId);

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null, null, null));

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 7, 100m, to.AddDays(-1), null, null, "Dividend"));

        using HttpResponseMessage response =
            await GetComparisonAsync(portfolioId, from, to);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isDataComplete").GetBoolean());
        Assert.True(root.GetProperty("areBothReturnsAvailable").GetBoolean());

        decimal twr = root.GetProperty("timeWeightedReturn").GetDecimal();
        decimal xirr = root.GetProperty("moneyWeightedReturn").GetDecimal();

        Assert.InRange(twr, 0.0999m, 0.1001m);
        Assert.InRange(xirr, 0.0999m, 0.1001m);

        decimal spread = root.GetProperty("moneyWeightedMinusTimeWeighted").GetDecimal();
        Assert.InRange(spread, -0.0002m, 0.0002m);
    }

    /// <summary>
    /// EN: Verifies external contribution timing is reflected in both composed methodologies without recomputation in the comparison layer.
    /// FA: بررسی می‌کند زمان‌بندی آورده خارجی در هر دو روش ترکیب‌شده منعکس شود، بدون محاسبه مجدد در لایه مقایسه.
    /// </summary>
    [Fact]
    public async Task Comparison_Should_Report_Zero_Returns_When_Only_External_Flow_Changes_Nav()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("GBP", $"Comparison GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Comparison Flow Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Comparison Flow {Guid.NewGuid():N}");

        DateTimeOffset from = new(2025, 9, 10, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset contribution = from.AddMonths(6);

        await SetBaseCurrencyAsync(portfolioId, currencyId);

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null, null, null));

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 1, 500m, contribution, null, null, null));

        using HttpResponseMessage response =
            await GetComparisonAsync(portfolioId, from, to);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isDataComplete").GetBoolean());
        Assert.True(root.GetProperty("areBothReturnsAvailable").GetBoolean());
        Assert.Equal(1, root.GetProperty("externalFlowBoundaryCount").GetInt32());

        decimal twr = root.GetProperty("timeWeightedReturn").GetDecimal();
        decimal xirr = root.GetProperty("moneyWeightedReturn").GetDecimal();

        Assert.InRange(twr, -0.0000001m, 0.0000001m);
        Assert.InRange(xirr, -0.0000001m, 0.0000001m);
    }

    /// <summary>
    /// EN: Verifies missing historical FX keeps the comparison visible but marks both return measures unavailable.
    /// FA: بررسی می‌کند نبود FX تاریخی پاسخ مقایسه را حفظ کند اما هر دو معیار بازده را غیرقابل‌دسترس علامت بزند.
    /// </summary>
    [Fact]
    public async Task Comparison_Should_Report_Incomplete_When_Historical_Fx_Is_Missing()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string eurId = await CreateCurrencyAsync("EUR", $"Comparison Missing FX EUR {Guid.NewGuid():N}");
        string usdId = await CreateCurrencyAsync("USD", $"Comparison Missing FX USD {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Comparison Missing FX Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Comparison Missing FX {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 12, 31, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, eurId);

        await CreateCashAsync(new CashRequest(
            portfolioId, eurId, 1, 1000m, from.AddDays(-1), null, null, null));

        await CreateCashAsync(new CashRequest(
            portfolioId, usdId, 1, 100m, from.AddMonths(3), null, null, null));

        using HttpResponseMessage response =
            await GetComparisonAsync(portfolioId, from, to);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.False(root.GetProperty("isDataComplete").GetBoolean());
        Assert.False(root.GetProperty("areBothReturnsAvailable").GetBoolean());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("timeWeightedReturn").ValueKind);
        Assert.Equal(JsonValueKind.Null, root.GetProperty("moneyWeightedReturn").ValueKind);
        Assert.Equal(JsonValueKind.Null, root.GetProperty("moneyWeightedMinusTimeWeighted").ValueKind);
    }

    private async Task<HttpResponseMessage> GetComparisonAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/comparison" +
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
