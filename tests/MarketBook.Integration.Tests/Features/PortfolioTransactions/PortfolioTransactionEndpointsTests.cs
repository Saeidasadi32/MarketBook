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
/// EN: Integration tests for immutable portfolio transaction endpoints.
/// FA: تست‌های Integration مربوط به Endpointهای تراکنش تغییرناپذیر پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioTransactionEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes portfolio transaction endpoint tests.
    /// FA: تست‌های Endpoint تراکنش پرتفوی را مقداردهی می‌کند.
    /// </summary>
    public PortfolioTransactionEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    [Fact]
    public async Task Create_And_GetById_Should_Persist_Immutable_Transaction()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync("Transaction Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Trading");

        CreateTransactionRequest request = CreateRequest(
            portfolioId,
            listingId,
            1,
            12.5m,
            1234.56789m,
            10m);

        string transactionId = await CreateTransactionAsync(request);

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/portfolio-transactions/{transactionId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.Equal(portfolioId, root.GetProperty("portfolioId").GetString());
        Assert.Equal(listingId, root.GetProperty("listingId").GetString());
        Assert.Equal(1, root.GetProperty("type").GetInt32());
        Assert.Equal(12.5m, root.GetProperty("quantity").GetDecimal());
        Assert.Equal(1234.56789m, root.GetProperty("price").GetDecimal());
        Assert.Equal(10m, root.GetProperty("commission").GetDecimal());
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("currencyId").GetString()));
    }

    [Fact]
    public async Task Create_WhenPortfolioInactive_Should_Return_BadRequest()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync("Inactive Portfolio Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Inactive Portfolio");

        using HttpResponseMessage deactivate =
            await _client.PatchAsync(
                $"/api/v1/portfolios/{portfolioId}/deactivate",
                null);

        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-transactions",
                CreateRequest(portfolioId, listingId, 1, 1m, 100m, 0m));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(
            response,
            "PortfolioTransaction.PortfolioInactive");
    }

    [Fact]
    public async Task Create_WhenListingMissing_Should_Return_NotFound()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string investorId = await CreateInvestorAsync("Missing Listing Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Missing Listing");

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-transactions",
                CreateRequest(
                    portfolioId,
                    "01ARZ3NDEKTSV4RRFFQ69G5FAV",
                    1,
                    1m,
                    100m,
                    0m));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(
            response,
            "PortfolioTransaction.ListingNotFound");
    }

    [Fact]
    public async Task Sell_Above_OpenQuantity_Should_Return_BadRequest()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync("Sell Guard Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Sell Guard");

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 1, 10m, 100m, 0m));

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-transactions",
                CreateRequest(portfolioId, listingId, 2, 10.0001m, 110m, 0m));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(
            response,
            "PortfolioTransaction.InsufficientQuantity");
    }

    [Fact]
    public async Task Positions_Should_Rebuild_Weighted_Average_From_Ledger()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync("Projection Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Projection");

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 1, 10m, 100m, 10m));

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 1, 10m, 200m, 10m));

        await CreateTransactionAsync(
            CreateRequest(portfolioId, listingId, 2, 5m, 250m, 5m));

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions/positions?portfolioId={portfolioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] items =
            document.RootElement
                .GetProperty("items")
                .EnumerateArray()
                .ToArray();

        Assert.Single(items);
        Assert.Equal(listingId, items[0].GetProperty("listingId").GetString());
        Assert.Equal(15m, items[0].GetProperty("quantity").GetDecimal());
        Assert.Equal(151m, items[0].GetProperty("averageAcquisitionPrice").GetDecimal());
        Assert.Equal(2265m, items[0].GetProperty("investedCost").GetDecimal());
    }

    [Fact]
    public async Task GetAll_Should_Order_Newest_First_And_Normalize_Pagination()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string investorId = await CreateInvestorAsync("Paged Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Paged");

        DateTimeOffset firstTime = new(2026, 9, 8, 8, 0, 0, TimeSpan.Zero);
        DateTimeOffset secondTime = new(2026, 9, 8, 9, 0, 0, TimeSpan.Zero);

        string firstId = await CreateTransactionAsync(
            CreateRequest(
                portfolioId,
                listingId,
                1,
                1m,
                100m,
                0m,
                firstTime));

        string secondId = await CreateTransactionAsync(
            CreateRequest(
                portfolioId,
                listingId,
                1,
                1m,
                110m,
                0m,
                secondTime));

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions?portfolioId={portfolioId}&page=0&pageSize=500");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.Equal(1, root.GetProperty("page").GetInt32());
        Assert.Equal(100, root.GetProperty("pageSize").GetInt32());
        Assert.Equal(2, root.GetProperty("totalCount").GetInt32());

        JsonElement[] items =
            root.GetProperty("items").EnumerateArray().ToArray();

        Assert.Equal(secondId, items[0].GetProperty("id").GetString());
        Assert.Equal(firstId, items[1].GetProperty("id").GetString());
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

    private async Task<string> CreatePortfolioAsync(
        string investorId,
        string name)
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

    private async Task<string> CreateTransactionAsync(
        CreateTransactionRequest request)
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
        decimal commission,
        DateTimeOffset? executedOn = null)
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
            executedOn ?? DateTimeOffset.UtcNow);

    private static async Task AssertProblemCodeAsync(
        HttpResponseMessage response,
        string expectedCode)
    {
        using JsonDocument document = await ReadJsonAsync(response);

        Assert.Equal(
            expectedCode,
            document.RootElement.GetProperty("code").GetString());
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record InvestorRequest(string FullName);

    private sealed record PortfolioRequest(
        string InvestorId,
        string Name);

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
