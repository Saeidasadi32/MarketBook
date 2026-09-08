// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.PortfolioTransactions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.PortfolioTransactions;

/// <summary>
/// EN: Integration tests for portfolio valuation and unrealized P/L.
/// FA: تست‌های Integration مربوط به ارزش‌گذاری پرتفوی و سود/زیان تحقق‌نیافته.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioValuationEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes portfolio valuation endpoint tests.
    /// FA: تست‌های Endpoint ارزش‌گذاری پرتفوی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: fixture تست یکپارچه.</param>
    public PortfolioValuationEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies valuation uses the latest available last-traded market price.
    /// FA: بررسی می‌کند ارزش‌گذاری از آخرین قیمت معامله‌شده موجود استفاده کند.
    /// </summary>
    [Fact]
    public async Task Valuation_Should_Use_Latest_MarketPrice_And_Calculate_UnrealizedPnl()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync($"Valuation Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Valuation {Guid.NewGuid():N}");

        await CreateTransactionAsync(
            CreateTransactionRequest(
                portfolioId,
                listingId,
                1,
                10m,
                100m,
                10m));

        await CreateMarketPriceAsync(
            listingId,
            new DateOnly(2026, 9, 7),
            120m);

        await CreateMarketPriceAsync(
            listingId,
            new DateOnly(2026, 9, 8),
            130m);

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions/valuation?portfolioId={portfolioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement item =
            document.RootElement.GetProperty("items").EnumerateArray().Single();

        Assert.Equal(listingId, item.GetProperty("listingId").GetString());
        Assert.Equal(10m, item.GetProperty("quantity").GetDecimal());
        Assert.Equal(101m, item.GetProperty("averageAcquisitionPrice").GetDecimal());
        Assert.Equal(1010m, item.GetProperty("investedCost").GetDecimal());
        Assert.True(item.GetProperty("isPriced").GetBoolean());
        Assert.Equal("2026-09-08", item.GetProperty("marketPriceDate").GetString());
        Assert.Equal(130m, item.GetProperty("marketPrice").GetDecimal());
        Assert.Equal(1300m, item.GetProperty("marketValue").GetDecimal());
        Assert.Equal(290m, item.GetProperty("unrealizedProfitLoss").GetDecimal());
        Assert.Equal(
            290m / 1010m * 100m,
            item.GetProperty("unrealizedReturnPercent").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies an open position remains visible when no market price exists.
    /// FA: بررسی می‌کند موقعیت باز در نبود قیمت بازار همچنان در خروجی باقی بماند.
    /// </summary>
    [Fact]
    public async Task Valuation_When_MarketPrice_Is_Missing_Should_Return_Unpriced_Position()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync($"Unpriced Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Unpriced {Guid.NewGuid():N}");

        await CreateTransactionAsync(
            CreateTransactionRequest(
                portfolioId,
                listingId,
                1,
                4m,
                50m,
                0m));

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions/valuation?portfolioId={portfolioId}&listingId={listingId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement item =
            document.RootElement.GetProperty("items").EnumerateArray().Single();

        Assert.False(item.GetProperty("isPriced").GetBoolean());
        Assert.Equal(JsonValueKind.Null, item.GetProperty("marketPriceDate").ValueKind);
        Assert.Equal(JsonValueKind.Null, item.GetProperty("marketPrice").ValueKind);
        Assert.Equal(JsonValueKind.Null, item.GetProperty("marketValue").ValueKind);
        Assert.Equal(JsonValueKind.Null, item.GetProperty("unrealizedProfitLoss").ValueKind);
        Assert.Equal(JsonValueKind.Null, item.GetProperty("unrealizedReturnPercent").ValueKind);
    }

    /// <summary>
    /// EN: Verifies sells reduce invested cost at moving weighted-average acquisition cost.
    /// FA: بررسی می‌کند فروش، بهای سرمایه‌گذاری‌شده را بر مبنای میانگین موزون متحرک کاهش دهد.
    /// </summary>
    [Fact]
    public async Task Valuation_Should_Relieve_BookCost_At_WeightedAverage_After_Sell()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync($"Sell Valuation Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Sell Valuation {Guid.NewGuid():N}");

        await CreateTransactionAsync(
            CreateTransactionRequest(portfolioId, listingId, 1, 10m, 100m, 0m));

        await CreateTransactionAsync(
            CreateTransactionRequest(portfolioId, listingId, 1, 10m, 200m, 0m));

        await CreateTransactionAsync(
            CreateTransactionRequest(portfolioId, listingId, 2, 5m, 250m, 0m));

        await CreateMarketPriceAsync(
            listingId,
            new DateOnly(2026, 9, 8),
            180m);

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions/valuation?portfolioId={portfolioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement item =
            document.RootElement.GetProperty("items").EnumerateArray().Single();

        Assert.Equal(15m, item.GetProperty("quantity").GetDecimal());
        Assert.Equal(150m, item.GetProperty("averageAcquisitionPrice").GetDecimal());
        Assert.Equal(2250m, item.GetProperty("investedCost").GetDecimal());
        Assert.Equal(2700m, item.GetProperty("marketValue").GetDecimal());
        Assert.Equal(450m, item.GetProperty("unrealizedProfitLoss").GetDecimal());
        Assert.Equal(20m, item.GetProperty("unrealizedReturnPercent").GetDecimal());
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

    private async Task CreateTransactionAsync(CreatePortfolioTransactionRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-transactions",
                request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task CreateMarketPriceAsync(
        string listingId,
        DateOnly tradingDate,
        decimal lastPrice)
    {
        decimal lowerLimit = lastPrice - 20m;
        decimal upperLimit = lastPrice + 20m;

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
            lowerLimit,
            upperLimit);

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/market-prices",
                request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static CreatePortfolioTransactionRequest CreateTransactionRequest(
        string portfolioId,
        string listingId,
        int type,
        decimal quantity,
        decimal price,
        decimal commission)
        => new(
            portfolioId,
            listingId,
            type,
            quantity,
            price,
            commission,
            0m,
            0m,
            0m,
            0m,
            0m,
            DateTimeOffset.UtcNow);

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record InvestorRequest(string FullName);

    private sealed record PortfolioRequest(string InvestorId, string Name);

    private sealed record CreatePortfolioTransactionRequest(
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
