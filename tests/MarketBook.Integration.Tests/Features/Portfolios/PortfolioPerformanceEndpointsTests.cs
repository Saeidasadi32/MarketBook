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
/// EN: Integration tests for the historical portfolio performance foundation.
/// FA: تست‌های Integration مربوط به مبنای عملکرد تاریخی پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioPerformanceEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes portfolio performance endpoint tests.
    /// FA: تست‌های Endpoint عملکرد پرتفوی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioPerformanceEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies external flow period semantics are (From, To] and avoid double counting the beginning boundary.
    /// FA: بررسی می‌کند مرز جریان خارجی (From, To] باشد و جریان دقیقاً روی ابتدای دوره دوباره‌شماری نشود.
    /// </summary>
    [Fact]
    public async Task Performance_Should_Count_External_Flows_Only_After_From_Through_To()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string baseCurrencyId = await CreateCurrencyAsync("EUR", $"Performance EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Performance Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Performance {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, baseCurrencyId);

        await CreateCashAsync(new CashRequest(
            portfolioId, baseCurrencyId, 1, 1000m, from.AddDays(-1), null, null, null));

        await CreateCashAsync(new CashRequest(
            portfolioId, baseCurrencyId, 1, 200m, from, null, null, null));

        await CreateCashAsync(new CashRequest(
            portfolioId, baseCurrencyId, 1, 500m, from.AddDays(1), null, null, null));

        using HttpResponseMessage response =
            await GetPerformanceAsync(portfolioId, from, to);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(1200m, root.GetProperty("beginningNetAssetValueBase").GetDecimal());
        Assert.Equal(1700m, root.GetProperty("endingNetAssetValueBase").GetDecimal());
        Assert.Equal(500m, root.GetProperty("netExternalFlowBase").GetDecimal());
        Assert.Equal(0m, root.GetProperty("investmentProfitLossBase").GetDecimal());

        JsonElement[] flows = root.GetProperty("externalFlows").EnumerateArray().ToArray();
        Assert.Single(flows);
        Assert.Equal(500m, flows[0].GetProperty("signedSourceAmount").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies dividends remain investment result and are not classified as external capital flows.
    /// FA: بررسی می‌کند سود نقدی بخشی از نتیجه سرمایه‌گذاری بماند و جریان سرمایه خارجی محسوب نشود.
    /// </summary>
    [Fact]
    public async Task Performance_Should_Treat_Dividend_As_Investment_Result_Not_External_Flow()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string baseCurrencyId = await CreateCurrencyAsync("GBP", $"Performance GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Dividend Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Dividend Performance {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, baseCurrencyId);

        await CreateCashAsync(new CashRequest(
            portfolioId, baseCurrencyId, 1, 1000m, from.AddDays(-1), null, null, null));

        await CreateCashAsync(new CashRequest(
            portfolioId, baseCurrencyId, 7, 100m, from.AddDays(1), null, null, "Dividend"));

        using HttpResponseMessage response =
            await GetPerformanceAsync(portfolioId, from, to);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(1000m, root.GetProperty("beginningNetAssetValueBase").GetDecimal());
        Assert.Equal(1100m, root.GetProperty("endingNetAssetValueBase").GetDecimal());
        Assert.Equal(0m, root.GetProperty("netExternalFlowBase").GetDecimal());
        Assert.Equal(100m, root.GetProperty("investmentProfitLossBase").GetDecimal());
        Assert.Empty(root.GetProperty("externalFlows").EnumerateArray());
    }

    /// <summary>
    /// EN: Verifies a foreign external contribution uses FX available at the flow date while later FX movement remains investment result.
    /// FA: بررسی می‌کند آورده خارجی با FX تاریخ جریان ترجمه شود و تغییر بعدی FX در نتیجه سرمایه‌گذاری باقی بماند.
    /// </summary>
    [Fact]
    public async Task Performance_Should_Translate_External_Flow_At_Occurrence_Date_And_Keep_Later_Fx_Move_In_Result()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string usdId = await GetListingQuoteCurrencyAsync(listingId);
        string eurId = await CreateCurrencyAsync("EUR", $"FX Performance EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"FX Performance Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"FX Performance {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 7, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset flowTime = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, eurId);

        await CreateCashAsync(new CashRequest(
            portfolioId, eurId, 1, 1000m, from.AddDays(-1), null, null, null));

        await CreateFxRateAsync(
            usdId,
            eurId,
            new DateOnly(2026, 9, 7),
            0.8m);

        await CreateCashAsync(new CashRequest(
            portfolioId, usdId, 1, 100m, flowTime, null, null, null));

        await CreateFxRateAsync(
            usdId,
            eurId,
            new DateOnly(2026, 9, 9),
            0.9m);

        using HttpResponseMessage response =
            await GetPerformanceAsync(portfolioId, from, to);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(1000m, root.GetProperty("beginningNetAssetValueBase").GetDecimal());
        Assert.Equal(1090m, root.GetProperty("endingNetAssetValueBase").GetDecimal());
        Assert.Equal(80m, root.GetProperty("netExternalFlowBase").GetDecimal());
        Assert.Equal(10m, root.GetProperty("investmentProfitLossBase").GetDecimal());

        JsonElement flow = root.GetProperty("externalFlows").EnumerateArray().Single();
        Assert.Equal("Deposit", flow.GetProperty("type").GetString());
        Assert.Equal(100m, flow.GetProperty("signedSourceAmount").GetDecimal());
        Assert.Equal(0.8m, flow.GetProperty("sourceToBaseRate").GetDecimal());
        Assert.Equal(80m, flow.GetProperty("signedAmountBase").GetDecimal());

        string? fxRateDateText = flow.GetProperty("fxRateDate").GetString();
        Assert.False(string.IsNullOrWhiteSpace(fxRateDateText));
        Assert.Equal(new DateOnly(2026, 9, 7), DateOnly.Parse(fxRateDateText));
    }

    private async Task<HttpResponseMessage> GetPerformanceAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance" +
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

    private async Task CreateFxRateAsync(
        string baseCurrencyId,
        string quoteCurrencyId,
        DateOnly rateDate,
        decimal rate)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/fx-rates",
                new FxRateRequest(
                    baseCurrencyId,
                    quoteCurrencyId,
                    rateDate,
                    rate));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record CurrencyRequest(string Code, string Name, int DecimalPlaces);
    private sealed record InvestorRequest(string FullName);
    private sealed record PortfolioRequest(string InvestorId, string Name);
    private sealed record SetBaseCurrencyRequest(string CurrencyId);
    private sealed record FxRateRequest(
        string BaseCurrencyId,
        string QuoteCurrencyId,
        DateOnly RateDate,
        decimal Rate);
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
