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
/// EN: Integration tests for current portfolio NAV by currency.
/// FA: تست‌های Integration مربوط به NAV جاری پرتفوی به تفکیک ارز.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioNavEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes portfolio NAV endpoint tests.
    /// FA: تست‌های Endpoint مربوط به NAV پرتفوی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioNavEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies NAV combines post-settlement cash with priced market value.
    /// FA: بررسی می‌کند NAV موجودی نقدی پس از تسویه را با ارزش بازار قیمت‌دار ترکیب کند.
    /// </summary>
    [Fact]
    public async Task Nav_Should_Combine_Cash_And_Priced_Position()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string currencyId = await GetListingQuoteCurrencyAsync(listingId);
        string investorId = await CreateInvestorAsync($"NAV Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"NAV {Guid.NewGuid():N}");

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                1,
                2000m,
                DateTimeOffset.UtcNow.AddMinutes(-2),
                null,
                null,
                "NAV test deposit"));

        await CreateTransactionAsync(
            new TradeRequest(
                portfolioId,
                listingId,
                1,
                10m,
                100m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                DateTimeOffset.UtcNow.AddMinutes(-1)));

        await CreateMarketPriceAsync(listingId, new DateOnly(2026, 9, 9), 130m);

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}/nav");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement item = root.GetProperty("currencies").EnumerateArray().Single();

        Assert.Equal(portfolioId, root.GetProperty("portfolioId").GetString());
        Assert.Equal(currencyId, item.GetProperty("currencyId").GetString());
        Assert.Equal(1000m, item.GetProperty("cashBalance").GetDecimal());
        Assert.Equal(1300m, item.GetProperty("pricedMarketValue").GetDecimal());
        Assert.Equal(2300m, item.GetProperty("pricedNetAssetValue").GetDecimal());
        Assert.True(item.GetProperty("isComplete").GetBoolean());
        Assert.Equal(2300m, item.GetProperty("netAssetValue").GetDecimal());
        Assert.Equal(1, item.GetProperty("pricedPositionCount").GetInt32());
        Assert.Equal(0, item.GetProperty("unpricedPositionCount").GetInt32());
    }

    /// <summary>
    /// EN: Verifies an unpriced open position makes complete NAV unavailable while preserving priced NAV.
    /// FA: بررسی می‌کند موقعیت باز بدون قیمت، NAV کامل را ناموجود کند ولی NAV مبتنی بر اقلام قیمت‌دار حفظ شود.
    /// </summary>
    [Fact]
    public async Task Nav_When_Position_Is_Unpriced_Should_Return_Incomplete_Nav()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string currencyId = await GetListingQuoteCurrencyAsync(listingId);
        string investorId = await CreateInvestorAsync($"NAV Unpriced Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"NAV Unpriced {Guid.NewGuid():N}");

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                1,
                1000m,
                DateTimeOffset.UtcNow.AddMinutes(-2),
                null,
                null,
                null));

        await CreateTransactionAsync(
            new TradeRequest(
                portfolioId,
                listingId,
                1,
                4m,
                50m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                DateTimeOffset.UtcNow.AddMinutes(-1)));

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}/nav");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement item =
            document.RootElement.GetProperty("currencies").EnumerateArray().Single();

        Assert.Equal(800m, item.GetProperty("cashBalance").GetDecimal());
        Assert.Equal(0m, item.GetProperty("pricedMarketValue").GetDecimal());
        Assert.Equal(800m, item.GetProperty("pricedNetAssetValue").GetDecimal());
        Assert.False(item.GetProperty("isComplete").GetBoolean());
        Assert.Equal(JsonValueKind.Null, item.GetProperty("netAssetValue").ValueKind);
        Assert.Equal(0, item.GetProperty("pricedPositionCount").GetInt32());
        Assert.Equal(1, item.GetProperty("unpricedPositionCount").GetInt32());
    }

    /// <summary>
    /// EN: Verifies different currencies remain separate and are never summed directly.
    /// FA: بررسی می‌کند ارزهای متفاوت مستقل باقی بمانند و مستقیماً با یکدیگر جمع نشوند.
    /// </summary>
    [Fact]
    public async Task Nav_Should_Keep_Multiple_Currencies_Separate()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string investorId = await CreateInvestorAsync($"NAV Multi Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"NAV Multi {Guid.NewGuid():N}");
        string usdId = await CreateCurrencyAsync("USD", $"US Dollar NAV {Guid.NewGuid():N}");
        string eurId = await CreateCurrencyAsync("EUR", $"Euro NAV {Guid.NewGuid():N}");

        await CreateCashAsync(new CashRequest(portfolioId, usdId, 1, 1000m, DateTimeOffset.UtcNow, null, null, null));
        await CreateCashAsync(new CashRequest(portfolioId, eurId, 1, 500m, DateTimeOffset.UtcNow.AddSeconds(1), null, null, null));

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}/nav");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] items =
            document.RootElement.GetProperty("currencies").EnumerateArray().ToArray();

        Assert.Equal(2, items.Length);

        JsonElement usd = items.Single(item => item.GetProperty("currencyId").GetString() == usdId);
        JsonElement eur = items.Single(item => item.GetProperty("currencyId").GetString() == eurId);

        Assert.Equal(1000m, usd.GetProperty("netAssetValue").GetDecimal());
        Assert.Equal(500m, eur.GetProperty("netAssetValue").GetDecimal());
        Assert.True(usd.GetProperty("isComplete").GetBoolean());
        Assert.True(eur.GetProperty("isComplete").GetBoolean());
    }

    private async Task<string> CreateInvestorAsync(string fullName)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync("/api/v1/investors", new InvestorRequest(fullName));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();
        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> CreatePortfolioAsync(string investorId, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync("/api/v1/portfolios", new PortfolioRequest(investorId, name));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();
        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

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
            await _client.PostAsJsonAsync("/api/v1/portfolio-cash-transactions", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task CreateTransactionAsync(TradeRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync("/api/v1/portfolio-transactions", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task CreateMarketPriceAsync(
        string listingId,
        DateOnly tradingDate,
        decimal lastPrice)
    {
        MarketPriceRequest request = new(
            listingId,
            tradingDate,
            lastPrice,
            lastPrice + 5m,
            lastPrice - 5m,
            lastPrice,
            lastPrice,
            lastPrice - 1m,
            lastPrice,
            lastPrice - 20m,
            lastPrice + 20m);

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync("/api/v1/market-prices", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

    private sealed record InvestorRequest(string FullName);
    private sealed record PortfolioRequest(string InvestorId, string Name);
    private sealed record CurrencyRequest(string Code, string Name, int DecimalPlaces);
    private sealed record CashRequest(
        string PortfolioId,
        string CurrencyId,
        int Type,
        decimal Amount,
        DateTimeOffset OccurredOn,
        string? ReferenceType,
        string? ReferenceId,
        string? Description);
    private sealed record TradeRequest(
        string PortfolioId,
        string ListingId,
        int Type,
        decimal Quantity,
        decimal Price,
        decimal Commission,
        decimal Tax,
        decimal ExchangeFee,
        decimal BrokerFee,
        decimal ClearingFee,
        decimal OtherFees,
        DateTimeOffset ExecutedOn);
    private sealed record MarketPriceRequest(
        string ListingId,
        DateOnly TradingDate,
        decimal OpenPrice,
        decimal HighPrice,
        decimal LowPrice,
        decimal LastPrice,
        decimal ClosePrice,
        decimal PreviousClosePrice,
        decimal ReferencePrice,
        decimal LowerLimit,
        decimal UpperLimit);
}
