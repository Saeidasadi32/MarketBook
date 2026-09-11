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

/// <summary>EN: Integration tests for historical TWR. FA: تست‌های Integration برای TWR تاریخی.</summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioTimeWeightedReturnEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>EN: Initializes the tests. FA: تست‌ها را مقداردهی می‌کند.</summary>
    public PortfolioTimeWeightedReturnEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    [Fact]
    public async Task Twr_Without_External_Flows_Should_Return_Single_Segment_Return()
    {
        await _fixture.ResetPortfolioTransactionsAsync();
        string currencyId = await CreateCurrencyAsync("EUR", $"TWR EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"TWR Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"TWR {Guid.NewGuid():N}");
        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);
        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null, null, null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, from.AddDays(1), null, null, "Dividend"));
        using HttpResponseMessage response = await GetTwrAsync(portfolioId, from, to);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.True(root.GetProperty("isCalculable").GetBoolean());
        Assert.Equal(0.1m, root.GetProperty("timeWeightedReturn").GetDecimal());
        Assert.Equal(0, root.GetProperty("externalFlowBoundaryCount").GetInt32());
        Assert.Single(root.GetProperty("segments").EnumerateArray());
    }

    [Fact]
    public async Task Twr_Should_Neutralize_External_Deposit()
    {
        await _fixture.ResetPortfolioTransactionsAsync();
        string currencyId = await CreateCurrencyAsync("GBP", $"TWR GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Deposit TWR Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Deposit TWR {Guid.NewGuid():N}");
        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset depositTime = new(2026, 9, 9, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);
        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null, null, null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, from.AddHours(12), null, null, "Dividend before"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, depositTime, null, null, null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 210m, depositTime.AddHours(12), null, null, "Dividend after"));
        using HttpResponseMessage response = await GetTwrAsync(portfolioId, from, to);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement[] segments = root.GetProperty("segments").EnumerateArray().ToArray();
        Assert.Equal(2, segments.Length);
        Assert.Equal(0.1m, segments[0].GetProperty("return").GetDecimal());
        Assert.Equal(0.1m, segments[1].GetProperty("return").GetDecimal());
        Assert.Equal(0.21m, root.GetProperty("timeWeightedReturn").GetDecimal());
    }

    [Fact]
    public async Task Twr_Should_Group_Multiple_External_Flows_At_Same_Instant()
    {
        await _fixture.ResetPortfolioTransactionsAsync();
        string currencyId = await CreateCurrencyAsync("CHF", $"TWR CHF {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Grouped TWR Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Grouped TWR {Guid.NewGuid():N}");
        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset flowTime = new(2026, 9, 9, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = new(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);
        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null, null, null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 500m, flowTime, null, null, null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 2, 200m, flowTime, null, null, null));
        using HttpResponseMessage response = await GetTwrAsync(portfolioId, from, to);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        Assert.Equal(1, root.GetProperty("externalFlowBoundaryCount").GetInt32());
        Assert.Equal(2, root.GetProperty("segments").EnumerateArray().Count());
        Assert.Equal(0m, root.GetProperty("timeWeightedReturn").GetDecimal());
    }

    private async Task<HttpResponseMessage> GetTwrAsync(string portfolioId, DateTimeOffset from, DateTimeOffset to)
        => await _client.GetAsync($"/api/v1/portfolios/{portfolioId}/twr?from={Uri.EscapeDataString(from.ToString("O"))}&to={Uri.EscapeDataString(to.ToString("O"))}");

    private async Task<string> CreateCurrencyAsync(string code, string name)
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/currencies", new CurrencyRequest(code, name, 2));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task<string> CreateInvestorAsync(string fullName)
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/investors", new InvestorRequest(fullName));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task<string> CreatePortfolioAsync(string investorId, string name)
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/portfolios", new PortfolioRequest(investorId, name));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task SetBaseCurrencyAsync(string portfolioId, string currencyId)
    {
        using HttpResponseMessage response = await _client.PatchAsJsonAsync($"/api/v1/portfolios/{portfolioId}/base-currency", new SetBaseCurrencyRequest(currencyId));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task CreateCashAsync(CashRequest request)
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/portfolio-cash-transactions", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

    private sealed record CurrencyRequest(string Code, string Name, int DecimalPlaces);
    private sealed record InvestorRequest(string FullName);
    private sealed record PortfolioRequest(string InvestorId, string Name);
    private sealed record SetBaseCurrencyRequest(string CurrencyId);
    private sealed record CashRequest(string PortfolioId, string CurrencyId, int Type, decimal Amount, DateTimeOffset OccurredOn, string? ReferenceType, string? ReferenceId, string? Description);
}
