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
/// EN: Integration tests for historical/as-of portfolio NAV.
/// FA: تست‌های Integration مربوط به NAV تاریخی/As-Of پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioNavAsOfEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes historical NAV endpoint tests.
    /// FA: تست‌های Endpoint مربوط به NAV تاریخی را مقداردهی می‌کند.
    /// </summary>
    public PortfolioNavAsOfEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies ledger entries after the cutoff do not affect historical NAV.
    /// FA: بررسی می‌کند سطرهای Ledger پس از لحظه برش بر NAV تاریخی اثر نگذارند.
    /// </summary>
    [Fact]
    public async Task NavAsOf_Should_Ignore_Future_Cash_And_Trades()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string currencyId = await GetListingQuoteCurrencyAsync(listingId);
        string investorId = await CreateInvestorAsync($"AsOf Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"AsOf {Guid.NewGuid():N}");

        DateTimeOffset cutoff = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 1, 1000m,
            cutoff.AddHours(-2), null, null, "before cutoff"));

        await CreateTransactionAsync(new TradeRequest(
            portfolioId, listingId, 1, 2m, 100m,
            0m, 0m, 0m, 0m, 0m, 0m,
            cutoff.AddHours(-1)));

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 1, 500m,
            cutoff.AddHours(1), null, null, "after cutoff"));

        await CreateTransactionAsync(new TradeRequest(
            portfolioId, listingId, 1, 1m, 100m,
            0m, 0m, 0m, 0m, 0m, 0m,
            cutoff.AddHours(2)));

        await CreateMarketPriceAsync(listingId, new DateOnly(2026, 9, 10), 150m);

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolios/{portfolioId}/nav/as-of?asOf={Uri.EscapeDataString(cutoff.ToString("O"))}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement item = root.GetProperty("currencies").EnumerateArray().Single();

        Assert.Equal(portfolioId, root.GetProperty("portfolioId").GetString());
        Assert.Equal(800m, item.GetProperty("cashBalance").GetDecimal());
        Assert.Equal(300m, item.GetProperty("pricedMarketValue").GetDecimal());
        Assert.Equal(1100m, item.GetProperty("netAssetValue").GetDecimal());
        Assert.Equal(1, item.GetProperty("pricedPositionCount").GetInt32());
        Assert.Equal(0, item.GetProperty("unpricedPositionCount").GetInt32());
    }

    /// <summary>
    /// EN: Verifies historical valuation uses the latest market price on or before the cutoff date.
    /// FA: بررسی می‌کند ارزش‌گذاری تاریخی آخرین قیمت بازار در تاریخ برش یا قبل از آن را استفاده کند.
    /// </summary>
    [Fact]
    public async Task NavAsOf_Should_Use_Latest_Price_On_Or_Before_Cutoff_Date()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string currencyId = await GetListingQuoteCurrencyAsync(listingId);
        string investorId = await CreateInvestorAsync($"AsOf Price Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"AsOf Price {Guid.NewGuid():N}");

        DateTimeOffset cutoff = new(2026, 9, 9, 23, 0, 0, TimeSpan.Zero);

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 1, 1000m,
            cutoff.AddDays(-2), null, null, null));

        await CreateTransactionAsync(new TradeRequest(
            portfolioId, listingId, 1, 2m, 50m,
            0m, 0m, 0m, 0m, 0m, 0m,
            cutoff.AddDays(-1)));

        await CreateMarketPriceAsync(listingId, new DateOnly(2026, 9, 8), 100m);
        await CreateMarketPriceAsync(listingId, new DateOnly(2026, 9, 10), 200m);

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolios/{portfolioId}/nav/as-of?asOf={Uri.EscapeDataString(cutoff.ToString("O"))}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement item = document.RootElement.GetProperty("currencies").EnumerateArray().Single();

        Assert.Equal(900m, item.GetProperty("cashBalance").GetDecimal());
        Assert.Equal(200m, item.GetProperty("pricedMarketValue").GetDecimal());
        Assert.Equal(1100m, item.GetProperty("netAssetValue").GetDecimal());
        Assert.True(item.GetProperty("isComplete").GetBoolean());
    }

    /// <summary>
    /// EN: Verifies a later market price is not backfilled into an earlier historical NAV.
    /// FA: بررسی می‌کند قیمت بازار آینده به NAV تاریخی قبل از آن تزریق نشود.
    /// </summary>
    [Fact]
    public async Task NavAsOf_When_No_Eligible_Price_Should_Return_Incomplete_Nav()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string currencyId = await GetListingQuoteCurrencyAsync(listingId);
        string investorId = await CreateInvestorAsync($"AsOf Unpriced Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"AsOf Unpriced {Guid.NewGuid():N}");

        DateTimeOffset cutoff = new(2026, 9, 9, 12, 0, 0, TimeSpan.Zero);

        await CreateCashAsync(new CashRequest(
            portfolioId, currencyId, 1, 1000m,
            cutoff.AddDays(-2), null, null, null));

        await CreateTransactionAsync(new TradeRequest(
            portfolioId, listingId, 1, 4m, 50m,
            0m, 0m, 0m, 0m, 0m, 0m,
            cutoff.AddDays(-1)));

        await CreateMarketPriceAsync(listingId, new DateOnly(2026, 9, 10), 130m);

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolios/{portfolioId}/nav/as-of?asOf={Uri.EscapeDataString(cutoff.ToString("O"))}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement item = document.RootElement.GetProperty("currencies").EnumerateArray().Single();

        Assert.Equal(800m, item.GetProperty("cashBalance").GetDecimal());
        Assert.Equal(0m, item.GetProperty("pricedMarketValue").GetDecimal());
        Assert.Equal(800m, item.GetProperty("pricedNetAssetValue").GetDecimal());
        Assert.False(item.GetProperty("isComplete").GetBoolean());
        Assert.Equal(JsonValueKind.Null, item.GetProperty("netAssetValue").ValueKind);
        Assert.Equal(0, item.GetProperty("pricedPositionCount").GetInt32());
        Assert.Equal(1, item.GetProperty("unpricedPositionCount").GetInt32());
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
