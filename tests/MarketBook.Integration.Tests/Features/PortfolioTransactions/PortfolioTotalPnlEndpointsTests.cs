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
/// EN: Integration tests for combined realized, unrealized, and total portfolio P/L.
/// FA: تست‌های Integration مربوط به سود/زیان تحقق‌یافته، تحقق‌نیافته و کل پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioTotalPnlEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes total portfolio P/L endpoint tests.
    /// FA: تست‌های Endpoint سود/زیان کل پرتفوی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: fixture تست یکپارچه.</param>
    public PortfolioTotalPnlEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies total P/L combines realized and unrealized P/L for a priced open position.
    /// FA: بررسی می‌کند سود/زیان کل برای موقعیت باز قیمت‌گذاری‌شده از جمع تحقق‌یافته و تحقق‌نیافته تشکیل شود.
    /// </summary>
    [Fact]
    public async Task TotalPnl_Should_Combine_Realized_And_Unrealized_Pnl()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync($"Total PnL Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Total PnL {Guid.NewGuid():N}");

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
                $"/api/v1/portfolio-transactions/total-pnl?portfolioId={portfolioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement item = root.GetProperty("items").EnumerateArray().Single();
        JsonElement currency = root.GetProperty("currencies").EnumerateArray().Single();

        Assert.Equal(500m, item.GetProperty("realizedProfitLoss").GetDecimal());
        Assert.Equal(15m, item.GetProperty("openQuantity").GetDecimal());
        Assert.Equal(2250m, item.GetProperty("remainingBookCost").GetDecimal());
        Assert.Equal(180m, item.GetProperty("marketPrice").GetDecimal());
        Assert.Equal(2700m, item.GetProperty("marketValue").GetDecimal());
        Assert.Equal(450m, item.GetProperty("unrealizedProfitLoss").GetDecimal());
        Assert.Equal(950m, item.GetProperty("totalProfitLoss").GetDecimal());

        Assert.True(currency.GetProperty("isFullyPriced").GetBoolean());
        Assert.Equal(500m, currency.GetProperty("realizedProfitLoss").GetDecimal());
        Assert.Equal(450m, currency.GetProperty("unrealizedProfitLoss").GetDecimal());
        Assert.Equal(950m, currency.GetProperty("totalProfitLoss").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies incomplete market pricing never produces a misleading currency total P/L.
    /// FA: بررسی می‌کند نبود قیمت کامل بازار هرگز سود/زیان کل ارزی گمراه‌کننده تولید نکند.
    /// </summary>
    [Fact]
    public async Task TotalPnl_When_Open_Position_Is_Unpriced_Should_Keep_Realized_But_Null_Total()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync($"Unpriced Total Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Unpriced Total {Guid.NewGuid():N}");

        await CreateTransactionAsync(
            CreateTransactionRequest(portfolioId, listingId, 1, 10m, 100m, 0m));

        await CreateTransactionAsync(
            CreateTransactionRequest(portfolioId, listingId, 2, 5m, 150m, 0m));

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions/total-pnl?portfolioId={portfolioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement item = document.RootElement.GetProperty("items").EnumerateArray().Single();
        JsonElement currency = document.RootElement.GetProperty("currencies").EnumerateArray().Single();

        Assert.Equal(250m, item.GetProperty("realizedProfitLoss").GetDecimal());
        Assert.Equal(5m, item.GetProperty("openQuantity").GetDecimal());
        Assert.False(item.GetProperty("isPriced").GetBoolean());
        Assert.Equal(JsonValueKind.Null, item.GetProperty("unrealizedProfitLoss").ValueKind);
        Assert.Equal(JsonValueKind.Null, item.GetProperty("totalProfitLoss").ValueKind);

        Assert.False(currency.GetProperty("isFullyPriced").GetBoolean());
        Assert.Equal(250m, currency.GetProperty("realizedProfitLoss").GetDecimal());
        Assert.Equal(JsonValueKind.Null, currency.GetProperty("unrealizedProfitLoss").ValueKind);
        Assert.Equal(JsonValueKind.Null, currency.GetProperty("totalProfitLoss").ValueKind);
    }

    /// <summary>
    /// EN: Verifies a fully closed position reports realized P/L as total P/L without requiring a market price.
    /// FA: بررسی می‌کند موقعیت کاملاً بسته بدون نیاز به قیمت بازار، سود/زیان تحقق‌یافته را به‌عنوان سود/زیان کل گزارش کند.
    /// </summary>
    [Fact]
    public async Task TotalPnl_For_Closed_Position_Should_Equal_Realized_Pnl()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync($"Closed Total Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Closed Total {Guid.NewGuid():N}");

        await CreateTransactionAsync(
            CreateTransactionRequest(portfolioId, listingId, 1, 4m, 100m, 4m));

        await CreateTransactionAsync(
            CreateTransactionRequest(portfolioId, listingId, 2, 4m, 150m, 6m));

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions/total-pnl?portfolioId={portfolioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement item = document.RootElement.GetProperty("items").EnumerateArray().Single();
        JsonElement currency = document.RootElement.GetProperty("currencies").EnumerateArray().Single();

        Assert.Equal(190m, item.GetProperty("realizedProfitLoss").GetDecimal());
        Assert.Equal(0m, item.GetProperty("openQuantity").GetDecimal());
        Assert.True(item.GetProperty("isPriced").GetBoolean());
        Assert.Equal(0m, item.GetProperty("unrealizedProfitLoss").GetDecimal());
        Assert.Equal(190m, item.GetProperty("totalProfitLoss").GetDecimal());

        Assert.True(currency.GetProperty("isFullyPriced").GetBoolean());
        Assert.Equal(190m, currency.GetProperty("realizedProfitLoss").GetDecimal());
        Assert.Equal(0m, currency.GetProperty("unrealizedProfitLoss").GetDecimal());
        Assert.Equal(190m, currency.GetProperty("totalProfitLoss").GetDecimal());
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
        MarketPriceRequest request = new(
            listingId,
            tradingDate,
            lastPrice,
            lastPrice,
            lastPrice,
            lastPrice,
            lastPrice,
            lastPrice,
            lastPrice,
            lastPrice - 20m,
            lastPrice + 20m);

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
