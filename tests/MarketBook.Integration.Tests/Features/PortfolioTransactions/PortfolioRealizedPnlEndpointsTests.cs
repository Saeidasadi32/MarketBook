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
/// EN: Integration tests for realized portfolio P/L projection.
/// FA: تست‌های Integration مربوط به تصویر سود/زیان تحقق‌یافته پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioRealizedPnlEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes realized P/L endpoint tests.
    /// FA: تست‌های Endpoint سود/زیان تحقق‌یافته را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: fixture تست یکپارچه.</param>
    public PortfolioRealizedPnlEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies weighted-average realized P/L including buy and sell costs.
    /// FA: سود/زیان تحقق‌یافته میانگین موزون را با احتساب هزینه‌های خرید و فروش بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task RealizedPnl_Should_Use_WeightedAverage_And_Include_Trade_Costs()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync("Realized PnL Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Realized PnL");

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 1, 10m, 100m, 10m));

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 1, 10m, 200m, 10m));

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 2, 5m, 250m, 5m));

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions/realized-pnl?portfolioId={portfolioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement[] items = root.GetProperty("items").EnumerateArray().ToArray();

        Assert.Equal(1, root.GetProperty("costBasisMethod").GetInt32());
        Assert.Single(items);
        Assert.Equal(listingId, items[0].GetProperty("listingId").GetString());
        Assert.Equal(5m, items[0].GetProperty("soldQuantity").GetDecimal());
        Assert.Equal(1250m, items[0].GetProperty("grossProceeds").GetDecimal());
        Assert.Equal(5m, items[0].GetProperty("sellingCosts").GetDecimal());
        Assert.Equal(1245m, items[0].GetProperty("netProceeds").GetDecimal());
        Assert.Equal(755m, items[0].GetProperty("costBasis").GetDecimal());
        Assert.Equal(490m, items[0].GetProperty("realizedProfitLoss").GetDecimal());
        Assert.Equal(15m, items[0].GetProperty("remainingQuantity").GetDecimal());
        Assert.Equal(2265m, items[0].GetProperty("remainingBookCost").GetDecimal());
        Assert.Equal(151m, items[0].GetProperty("remainingAverageCost").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies that a later buy recalculates moving weighted-average cost.
    /// FA: بررسی می‌کند خرید بعدی میانگین موزون متحرک بهای خرید را دوباره محاسبه کند.
    /// </summary>
    [Fact]
    public async Task RealizedPnl_Should_Recalculate_Average_After_Additional_Buy()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync("Moving Average Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Moving Average");

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 1, 10m, 100m, 0m));

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 2, 5m, 150m, 0m));

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 1, 5m, 200m, 0m));

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 2, 5m, 180m, 0m));

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions/realized-pnl?portfolioId={portfolioId}&listingId={listingId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement item =
            document.RootElement.GetProperty("items").EnumerateArray().Single();

        Assert.Equal(10m, item.GetProperty("soldQuantity").GetDecimal());
        Assert.Equal(1650m, item.GetProperty("grossProceeds").GetDecimal());
        Assert.Equal(1250m, item.GetProperty("costBasis").GetDecimal());
        Assert.Equal(400m, item.GetProperty("realizedProfitLoss").GetDecimal());
        Assert.Equal(5m, item.GetProperty("remainingQuantity").GetDecimal());
        Assert.Equal(750m, item.GetProperty("remainingBookCost").GetDecimal());
        Assert.Equal(150m, item.GetProperty("remainingAverageCost").GetDecimal());
    }

    /// <summary>
    /// EN: Rejects cost-basis methods not implemented by this foundation slice.
    /// FA: روش‌های بهای تمام‌شده پیاده‌سازی‌نشده در این فاز پایه را رد می‌کند.
    /// </summary>
    [Fact]
    public async Task RealizedPnl_WhenCostBasisMethodUnsupported_Should_Return_BadRequest()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string investorId = await CreateInvestorAsync("Unsupported Method Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Unsupported Method");

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions/realized-pnl?portfolioId={portfolioId}&costBasisMethod=99");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        Assert.Equal(
            "PortfolioTransaction.UnsupportedCostBasisMethod",
            document.RootElement.GetProperty("code").GetString());
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

    private async Task<string> CreateTransactionAsync(CreateTransactionRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-transactions",
                request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private static CreateTransactionRequest CreateRequest(
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

    private sealed record CreateTransactionRequest(
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
}
