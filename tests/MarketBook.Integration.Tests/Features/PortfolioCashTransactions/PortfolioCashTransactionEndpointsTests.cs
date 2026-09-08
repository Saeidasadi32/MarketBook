// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.PortfolioCashTransactions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.PortfolioCashTransactions;

/// <summary>EN: Integration tests for immutable portfolio cash-ledger endpoints. FA: تست‌های Integration مربوط به Endpointهای دفتر نقدی تغییرناپذیر پرتفوی.</summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioCashTransactionEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>EN: Initializes the test class. FA: کلاس تست را مقداردهی اولیه می‌کند.</summary>
    public PortfolioCashTransactionEndpointsTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>EN: Verifies create and read preserve immutable cash data. FA: صحت ثبت و خواندن داده تغییرناپذیر دفتر نقدی را بررسی می‌کند.</summary>
    [Fact]
    public async Task Create_And_GetById_Should_Persist_Cash_Transaction()
    {
        await _fixture.ResetPortfolioCashTransactionsAsync();
        string investorId = await CreateInvestorAsync("Cash Owner A");
        string portfolioId = await CreatePortfolioAsync(investorId, "Cash A");
        string currencyId = await CreateCurrencyAsync("XAA", "Cash Currency 01", 8);
        DateTimeOffset occurredOn = new(2026, 9, 8, 10, 0, 0, TimeSpan.Zero);

        string id = await CreateCashAsync(new(portfolioId, currencyId, 1, 1234.5678901234m, occurredOn, "Manual", "DEP-1", "Seed deposit"));
        using HttpResponseMessage response = await _client.GetAsync($"/api/v1/portfolio-cash-transactions/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        Assert.Equal(id, root.GetProperty("id").GetString());
        Assert.Equal(portfolioId, root.GetProperty("portfolioId").GetString());
        Assert.Equal(currencyId, root.GetProperty("currencyId").GetString());
        Assert.Equal(1234.5678901234m, root.GetProperty("amount").GetDecimal());
        Assert.Equal(1234.5678901234m, root.GetProperty("signedAmount").GetDecimal());
        Assert.Equal("DEP-1", root.GetProperty("referenceId").GetString());
    }

    /// <summary>EN: Verifies inactive portfolios reject cash entries. FA: رد تراکنش نقدی برای پرتفوی غیرفعال را بررسی می‌کند.</summary>
    [Fact]
    public async Task Create_WhenPortfolioInactive_Should_Return_BadRequest()
    {
        await _fixture.ResetPortfolioCashTransactionsAsync();
        string investorId = await CreateInvestorAsync("Cash Owner B");
        string portfolioId = await CreatePortfolioAsync(investorId, "Cash B");
        string currencyId = await CreateCurrencyAsync("XAB", "Cash Currency 02", 8);
        using HttpResponseMessage deactivate = await _client.PatchAsync($"/api/v1/portfolios/{portfolioId}/deactivate", null);
        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);
        using HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/portfolio-cash-transactions", new CashRequest(portfolioId, currencyId, 1, 10m, DateTimeOffset.UtcNow, null, null, null));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "PortfolioCashTransaction.PortfolioInactive");
    }

    /// <summary>EN: Verifies inactive currencies reject cash entries. FA: رد تراکنش نقدی برای ارز غیرفعال را بررسی می‌کند.</summary>
    [Fact]
    public async Task Create_WhenCurrencyInactive_Should_Return_BadRequest()
    {
        await _fixture.ResetPortfolioCashTransactionsAsync();
        string investorId = await CreateInvestorAsync("Cash Owner C");
        string portfolioId = await CreatePortfolioAsync(investorId, "Cash C");
        string currencyId = await CreateCurrencyAsync("XAC", "Cash Currency 03", 8);
        using HttpResponseMessage deactivate = await _client.PatchAsync($"/api/v1/currencies/{currencyId}/deactivate", null);
        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);
        using HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/portfolio-cash-transactions", new CashRequest(portfolioId, currencyId, 1, 10m, DateTimeOffset.UtcNow, null, null, null));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "PortfolioCashTransaction.CurrencyInactive");
    }

    /// <summary>EN: Verifies balance projection uses credit/debit semantics. FA: منطق بستانکار/بدهکار در محاسبه موجودی را بررسی می‌کند.</summary>
    [Fact]
    public async Task Balances_Should_Project_Credits_And_Debits_By_Currency()
    {
        await _fixture.ResetPortfolioCashTransactionsAsync();
        string investorId = await CreateInvestorAsync("Cash Owner D");
        string portfolioId = await CreatePortfolioAsync(investorId, "Cash D");
        string currency1 = await CreateCurrencyAsync("XAD", "Cash Currency 04", 8);
        string currency2 = await CreateCurrencyAsync("XAE", "Cash Currency 05", 8);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        await CreateCashAsync(new(portfolioId, currency1, 1, 1000m, now, null, null, null));
        await CreateCashAsync(new(portfolioId, currency1, 3, 250m, now.AddMinutes(1), null, null, null));
        await CreateCashAsync(new(portfolioId, currency1, 5, 10m, now.AddMinutes(2), null, null, null));
        await CreateCashAsync(new(portfolioId, currency1, 4, 400m, now.AddMinutes(3), null, null, null));
        await CreateCashAsync(new(portfolioId, currency2, 7, 25m, now.AddMinutes(4), null, null, null));

        using HttpResponseMessage response = await _client.GetAsync($"/api/v1/portfolio-cash-transactions/balances?portfolioId={portfolioId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement[] items = document.RootElement.GetProperty("items").EnumerateArray().ToArray();
        Assert.Equal(2, items.Length);
        JsonElement first = items.Single(item => item.GetProperty("currencyId").GetString() == currency1);
        JsonElement second = items.Single(item => item.GetProperty("currencyId").GetString() == currency2);
        Assert.Equal(1140m, first.GetProperty("balance").GetDecimal());
        Assert.Equal(25m, second.GetProperty("balance").GetDecimal());
    }

    /// <summary>EN: Verifies paged list ordering, normalization, and currency filter. FA: ترتیب، نرمال‌سازی صفحه‌بندی و فیلتر ارز را بررسی می‌کند.</summary>
    [Fact]
    public async Task GetAll_Should_Filter_Currency_Order_Newest_First_And_Normalize_Pagination()
    {
        await _fixture.ResetPortfolioCashTransactionsAsync();
        string investorId = await CreateInvestorAsync("Cash Owner E");
        string portfolioId = await CreatePortfolioAsync(investorId, "Cash E");
        string currency1 = await CreateCurrencyAsync("XAF", "Cash Currency 06", 8);
        string currency2 = await CreateCurrencyAsync("XAG", "Cash Currency 07", 8);
        DateTimeOffset firstTime = new(2026, 9, 8, 8, 0, 0, TimeSpan.Zero);
        DateTimeOffset secondTime = firstTime.AddHours(1);
        string firstId = await CreateCashAsync(new(portfolioId, currency1, 1, 10m, firstTime, null, null, null));
        string secondId = await CreateCashAsync(new(portfolioId, currency1, 1, 20m, secondTime, null, null, null));
        await CreateCashAsync(new(portfolioId, currency2, 1, 30m, secondTime.AddHours(1), null, null, null));

        using HttpResponseMessage response = await _client.GetAsync($"/api/v1/portfolio-cash-transactions?portfolioId={portfolioId}&currencyId={currency1}&page=0&pageSize=500");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        Assert.Equal(1, root.GetProperty("page").GetInt32());
        Assert.Equal(100, root.GetProperty("pageSize").GetInt32());
        Assert.Equal(2, root.GetProperty("totalCount").GetInt32());
        JsonElement[] items = root.GetProperty("items").EnumerateArray().ToArray();
        Assert.Equal(secondId, items[0].GetProperty("id").GetString());
        Assert.Equal(firstId, items[1].GetProperty("id").GetString());
    }

    /// <summary>EN: Verifies no update/delete endpoints exist for append-only ledger. FA: نبود Endpoint ویرایش/حذف برای دفتر append-only را بررسی می‌کند.</summary>
    [Fact]
    public async Task Ledger_Should_Not_Expose_Update_Or_Delete()
    {
        await _fixture.ResetPortfolioCashTransactionsAsync();
        string investorId = await CreateInvestorAsync("Cash Owner F");
        string portfolioId = await CreatePortfolioAsync(investorId, "Cash F");
        string currencyId = await CreateCurrencyAsync("XAH", "Cash Currency 08", 8);
        string id = await CreateCashAsync(new(portfolioId, currencyId, 1, 50m, DateTimeOffset.UtcNow, null, null, null));
        using HttpResponseMessage delete = await _client.DeleteAsync($"/api/v1/portfolio-cash-transactions/{id}");
        Assert.Equal(HttpStatusCode.MethodNotAllowed, delete.StatusCode);
    }

    private async Task<string> CreateInvestorAsync(string fullName)
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/investors", new InvestorRequest(fullName));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();
        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> CreatePortfolioAsync(string investorId, string name)
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/portfolios", new PortfolioRequest(investorId, name));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();
        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> CreateCurrencyAsync(string code, string name, int decimalPlaces)
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/currencies", new CurrencyRequest(code, name, decimalPlaces));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();
        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> CreateCashAsync(CashRequest request)
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/portfolio-cash-transactions", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();
        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private static async Task AssertProblemCodeAsync(HttpResponseMessage response, string expectedCode)
    {
        using JsonDocument document = await ReadJsonAsync(response);
        Assert.Equal(expectedCode, document.RootElement.GetProperty("code").GetString());
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
}
