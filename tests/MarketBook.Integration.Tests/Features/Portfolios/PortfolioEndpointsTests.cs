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
/// EN: Integration tests for Portfolio endpoints.
/// FA: تست‌های Integration مربوط به Endpointهای Portfolio.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes the tests.
    /// FA: تست‌ها را مقداردهی می‌کند.
    /// </summary>
    public PortfolioEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    [Fact]
    public async Task Create_And_GetById_Should_Succeed()
    {
        await _fixture.ResetInvestorsAsync();

        string investorId = await CreateInvestorAsync("Portfolio Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Growth");

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.Equal(investorId, root.GetProperty("investorId").GetString());
        Assert.Equal("Growth", root.GetProperty("name").GetString());
        Assert.True(root.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task Create_ForMissingInvestor_Should_Return_NotFound()
    {
        await _fixture.ResetInvestorsAsync();

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(
                    "01ARZ3NDEKTSV4RRFFQ69G5FAV",
                    "Missing Owner"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "Portfolio.InvestorNotFound");
    }

    [Fact]
    public async Task Create_ForInactiveInvestor_Should_Return_BadRequest()
    {
        await _fixture.ResetInvestorsAsync();

        string investorId = await CreateInvestorAsync("Inactive Owner");

        using HttpResponseMessage deactivate =
            await _client.PatchAsync(
                $"/api/v1/investors/{investorId}/deactivate",
                null);

        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(investorId, "Blocked"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "Portfolio.InvestorInactive");
    }

    [Fact]
    public async Task DuplicateName_ForSameInvestor_Should_Return_Conflict()
    {
        await _fixture.ResetInvestorsAsync();

        string investorId = await CreateInvestorAsync("Duplicate Owner");
        await CreatePortfolioAsync(investorId, "Main");

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(investorId, "Main"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "Portfolio.DuplicateName");
    }

    [Fact]
    public async Task SameName_ForDifferentInvestors_Should_Succeed()
    {
        await _fixture.ResetInvestorsAsync();

        string firstInvestorId = await CreateInvestorAsync("Owner One");
        string secondInvestorId = await CreateInvestorAsync("Owner Two");

        await CreatePortfolioAsync(firstInvestorId, "Main");
        await CreatePortfolioAsync(secondInvestorId, "Main");
    }

    [Fact]
    public async Task Update_Should_Persist_Name()
    {
        await _fixture.ResetInvestorsAsync();

        string investorId = await CreateInvestorAsync("Rename Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Before");

        using HttpResponseMessage update =
            await _client.PutAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}",
                new UpdateRequest("After"));

        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        using HttpResponseMessage get =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}");

        using JsonDocument document = await ReadJsonAsync(get);
        Assert.Equal("After", document.RootElement.GetProperty("name").GetString());
    }

    [Fact]
    public async Task Activate_And_Deactivate_Should_Be_Idempotent()
    {
        await _fixture.ResetInvestorsAsync();

        string investorId = await CreateInvestorAsync("Lifecycle Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Lifecycle");

        using HttpResponseMessage firstDeactivate =
            await _client.PatchAsync(
                $"/api/v1/portfolios/{portfolioId}/deactivate",
                null);

        using HttpResponseMessage secondDeactivate =
            await _client.PatchAsync(
                $"/api/v1/portfolios/{portfolioId}/deactivate",
                null);

        Assert.Equal(HttpStatusCode.OK, firstDeactivate.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondDeactivate.StatusCode);
        Assert.False(await GetPortfolioIsActiveAsync(portfolioId));

        using HttpResponseMessage firstActivate =
            await _client.PatchAsync(
                $"/api/v1/portfolios/{portfolioId}/activate",
                null);

        using HttpResponseMessage secondActivate =
            await _client.PatchAsync(
                $"/api/v1/portfolios/{portfolioId}/activate",
                null);

        Assert.Equal(HttpStatusCode.OK, firstActivate.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondActivate.StatusCode);
        Assert.True(await GetPortfolioIsActiveAsync(portfolioId));
    }

    [Fact]
    public async Task Activate_WhenInvestorInactive_Should_Return_BadRequest()
    {
        await _fixture.ResetInvestorsAsync();

        string investorId = await CreateInvestorAsync("Parent Lifecycle Owner");
        string portfolioId = await CreatePortfolioAsync(investorId, "Lifecycle");

        using HttpResponseMessage portfolioDeactivate =
            await _client.PatchAsync(
                $"/api/v1/portfolios/{portfolioId}/deactivate",
                null);

        Assert.Equal(HttpStatusCode.OK, portfolioDeactivate.StatusCode);

        using HttpResponseMessage investorDeactivate =
            await _client.PatchAsync(
                $"/api/v1/investors/{investorId}/deactivate",
                null);

        Assert.Equal(HttpStatusCode.OK, investorDeactivate.StatusCode);

        using HttpResponseMessage activate =
            await _client.PatchAsync(
                $"/api/v1/portfolios/{portfolioId}/activate",
                null);

        Assert.Equal(HttpStatusCode.BadRequest, activate.StatusCode);
        await AssertProblemCodeAsync(activate, "Portfolio.InvestorInactive");
    }

    [Fact]
    public async Task GetAll_Should_FilterByInvestor_And_NormalizePagination()
    {
        await _fixture.ResetInvestorsAsync();

        string firstInvestorId = await CreateInvestorAsync("Filter Owner One");
        string secondInvestorId = await CreateInvestorAsync("Filter Owner Two");

        await CreatePortfolioAsync(firstInvestorId, "C");
        await CreatePortfolioAsync(firstInvestorId, "A");
        await CreatePortfolioAsync(secondInvestorId, "B");

        using HttpResponseMessage filtered =
            await _client.GetAsync(
                $"/api/v1/portfolios?investorId={firstInvestorId}&page=1&pageSize=20");

        Assert.Equal(HttpStatusCode.OK, filtered.StatusCode);

        using JsonDocument filteredDocument = await ReadJsonAsync(filtered);
        JsonElement filteredRoot = filteredDocument.RootElement;

        Assert.Equal(2, filteredRoot.GetProperty("totalCount").GetInt32());
        Assert.Equal(
            2,
            filteredRoot.GetProperty("items").EnumerateArray().Count());

        using HttpResponseMessage normalized =
            await _client.GetAsync("/api/v1/portfolios?page=0&pageSize=500");

        Assert.Equal(HttpStatusCode.OK, normalized.StatusCode);

        using JsonDocument normalizedDocument = await ReadJsonAsync(normalized);
        Assert.Equal(1, normalizedDocument.RootElement.GetProperty("page").GetInt32());
        Assert.Equal(100, normalizedDocument.RootElement.GetProperty("pageSize").GetInt32());
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

    private async Task<bool> GetPortfolioIsActiveAsync(string portfolioId)
    {
        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}");

        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("isActive").GetBoolean();
    }

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

    private sealed record UpdateRequest(string Name);
}
