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
/// EN: Integration tests for automatic trade cash settlement.
/// FA: تست‌های Integration مربوط به تسویه نقدی خودکار معاملات.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioTransactionSettlementTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>EN: Initializes settlement tests. FA: تست‌های تسویه را مقداردهی اولیه می‌کند.</summary>
    public PortfolioTransactionSettlementTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    [Fact]
    public async Task CreateBuy_Should_Create_GrossPlusCosts_BuySettlement()
    {
        await _fixture.ResetPortfolioCashTransactionsAsync();
        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync("Settlement Buy Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Settlement Buy");

        CreateTransactionRequest request = new(
            portfolioId, listingId, 1, 2.5m, 100m,
            1m, 2m, 3m, 4m, 5m, 6m,
            new DateTimeOffset(2026, 9, 8, 12, 0, 0, TimeSpan.Zero));

        string transactionId = await CreateTransactionAsync(request);

        using HttpResponseMessage response = await _client.GetAsync(
            $"/api/v1/portfolio-cash-transactions?portfolioId={portfolioId}&page=1&pageSize=20");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] items = document.RootElement.GetProperty("items").EnumerateArray().ToArray();

        Assert.Single(items);
        Assert.Equal(3, items[0].GetProperty("type").GetInt32());
        Assert.Equal(271m, items[0].GetProperty("amount").GetDecimal());
        Assert.Equal("PortfolioTransaction", items[0].GetProperty("referenceType").GetString());
        Assert.Equal(transactionId, items[0].GetProperty("referenceId").GetString());
    }

    [Fact]
    public async Task CreateSell_Should_Create_GrossMinusCosts_SellSettlement()
    {
        await _fixture.ResetPortfolioCashTransactionsAsync();
        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync("Settlement Sell Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Settlement Sell");

        await CreateTransactionAsync(new(
            portfolioId, listingId, 1, 10m, 100m,
            0m, 0m, 0m, 0m, 0m, 0m, DateTimeOffset.UtcNow.AddMinutes(-1)));

        string sellId = await CreateTransactionAsync(new(
            portfolioId, listingId, 2, 4m, 150m,
            1m, 2m, 3m, 4m, 5m, 0m, DateTimeOffset.UtcNow));

        using HttpResponseMessage response = await _client.GetAsync(
            $"/api/v1/portfolio-cash-transactions?portfolioId={portfolioId}&page=1&pageSize=20");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] items = document.RootElement.GetProperty("items").EnumerateArray().ToArray();

        JsonElement sellSettlement = items.Single(
            item => item.GetProperty("referenceId").GetString() == sellId);

        Assert.Equal(4, sellSettlement.GetProperty("type").GetInt32());
        Assert.Equal(585m, sellSettlement.GetProperty("amount").GetDecimal());
    }

    [Fact]
    public async Task CreateSell_WhenCostsConsumeGross_Should_NotPersistSellOrSettlement()
    {
        await _fixture.ResetPortfolioCashTransactionsAsync();
        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync("Settlement Atomic Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Settlement Atomic");

        await CreateTransactionAsync(new(
            portfolioId, listingId, 1, 10m, 100m,
            0m, 0m, 0m, 0m, 0m, 0m, DateTimeOffset.UtcNow.AddMinutes(-1)));

        using HttpResponseMessage sellResponse = await _client.PostAsJsonAsync(
            "/api/v1/portfolio-transactions",
            new CreateTransactionRequest(
                portfolioId, listingId, 2, 1m, 10m,
                10m, 0m, 0m, 0m, 0m, 0m, DateTimeOffset.UtcNow));

        Assert.Equal(HttpStatusCode.BadRequest, sellResponse.StatusCode);
        using JsonDocument problem = await ReadJsonAsync(sellResponse);
        Assert.Equal(
            "PortfolioTransaction.InvalidSellSettlement",
            problem.RootElement.GetProperty("code").GetString());

        using HttpResponseMessage tradesResponse = await _client.GetAsync(
            $"/api/v1/portfolio-transactions?portfolioId={portfolioId}&page=1&pageSize=20");
        using JsonDocument trades = await ReadJsonAsync(tradesResponse);
        Assert.Equal(1, trades.RootElement.GetProperty("totalCount").GetInt32());

        using HttpResponseMessage cashResponse = await _client.GetAsync(
            $"/api/v1/portfolio-cash-transactions?portfolioId={portfolioId}&page=1&pageSize=20");
        using JsonDocument cash = await ReadJsonAsync(cashResponse);
        Assert.Equal(1, cash.RootElement.GetProperty("totalCount").GetInt32());
    }

    private async Task<string> CreateInvestorAsync(string fullName)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync("/api/v1/investors", new InvestorRequest(fullName));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task<string> CreatePortfolioAsync(string investorId, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync("/api/v1/portfolios", new PortfolioRequest(investorId, name));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task<string> CreateTransactionAsync(CreateTransactionRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync("/api/v1/portfolio-transactions", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

    private sealed record InvestorRequest(string FullName);
    private sealed record PortfolioRequest(string InvestorId, string Name);
    private sealed record CreateTransactionRequest(
        string PortfolioId, string ListingId, int Type, decimal Quantity, decimal Price,
        decimal Commission, decimal Tax, decimal ExchangeFee, decimal BrokerFee,
        decimal ClearingFee, decimal OtherFees, DateTimeOffset ExecutedOn);
}
