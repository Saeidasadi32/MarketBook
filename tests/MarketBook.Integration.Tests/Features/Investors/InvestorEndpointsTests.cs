// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.Investors
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.Investors;

/// <summary>EN: Integration tests for Investor endpoints. FA: تست‌های Integration مربوط به Endpointهای Investor.</summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class InvestorEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>EN: Initializes the tests. FA: تست‌ها را مقداردهی می‌کند.</summary>
    public InvestorEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    [Fact]
    public async Task Create_And_GetById_Should_Succeed()
    {
        await _fixture.ResetInvestorsAsync();

        string id = await CreateInvestorAsync("Investor Alpha");

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/investors/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        Assert.Equal("Investor Alpha", document.RootElement.GetProperty("fullName").GetString());
        Assert.True(document.RootElement.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task Create_InvalidName_Should_Return_BadRequest()
    {
        await _fixture.ResetInvestorsAsync();

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/investors",
                new InvestorRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "Investor.InvalidName");
    }

    [Fact]
    public async Task Update_Should_Persist_Name()
    {
        await _fixture.ResetInvestorsAsync();

        string id = await CreateInvestorAsync("Before");

        using HttpResponseMessage update =
            await _client.PutAsJsonAsync(
                $"/api/v1/investors/{id}",
                new InvestorRequest("After"));

        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        using HttpResponseMessage get =
            await _client.GetAsync($"/api/v1/investors/{id}");

        using JsonDocument document = await ReadJsonAsync(get);
        Assert.Equal("After", document.RootElement.GetProperty("fullName").GetString());
    }

    [Fact]
    public async Task Activate_And_Deactivate_Should_Be_Idempotent()
    {
        await _fixture.ResetInvestorsAsync();

        string id = await CreateInvestorAsync("Lifecycle Investor");

        using HttpResponseMessage firstDeactivate =
            await _client.PatchAsync($"/api/v1/investors/{id}/deactivate", null);
        using HttpResponseMessage secondDeactivate =
            await _client.PatchAsync($"/api/v1/investors/{id}/deactivate", null);

        Assert.Equal(HttpStatusCode.OK, firstDeactivate.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondDeactivate.StatusCode);
        Assert.False(await GetIsActiveAsync(id));

        using HttpResponseMessage firstActivate =
            await _client.PatchAsync($"/api/v1/investors/{id}/activate", null);
        using HttpResponseMessage secondActivate =
            await _client.PatchAsync($"/api/v1/investors/{id}/activate", null);

        Assert.Equal(HttpStatusCode.OK, firstActivate.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondActivate.StatusCode);
        Assert.True(await GetIsActiveAsync(id));
    }

    [Fact]
    public async Task GetAll_Should_Page_And_Normalize_Request()
    {
        await _fixture.ResetInvestorsAsync();

        await CreateInvestorAsync("Investor C");
        await CreateInvestorAsync("Investor A");
        await CreateInvestorAsync("Investor B");

        using HttpResponseMessage paged =
            await _client.GetAsync("/api/v1/investors?page=2&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, paged.StatusCode);

        using JsonDocument pagedDocument = await ReadJsonAsync(paged);
        JsonElement root = pagedDocument.RootElement;

        Assert.Equal(2, root.GetProperty("page").GetInt32());
        Assert.Equal(1, root.GetProperty("pageSize").GetInt32());
        Assert.Equal(3, root.GetProperty("totalCount").GetInt32());
        Assert.Single(root.GetProperty("items").EnumerateArray());

        using HttpResponseMessage normalized =
            await _client.GetAsync("/api/v1/investors?page=0&pageSize=500");

        using JsonDocument normalizedDocument = await ReadJsonAsync(normalized);
        Assert.Equal(1, normalizedDocument.RootElement.GetProperty("page").GetInt32());
        Assert.Equal(100, normalizedDocument.RootElement.GetProperty("pageSize").GetInt32());
    }

    [Theory]
    [InlineData("abc", HttpStatusCode.BadRequest, "Investor.InvalidId")]
    [InlineData("01ARZ3NDEKTSV4RRFFQ69G5FAV", HttpStatusCode.NotFound, "Investor.NotFound")]
    public async Task GetById_InvalidOrMissing_Should_Return_ExpectedError(
        string id,
        HttpStatusCode expectedStatus,
        string expectedCode)
    {
        await _fixture.ResetInvestorsAsync();

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/investors/{id}");

        Assert.Equal(expectedStatus, response.StatusCode);
        await AssertProblemCodeAsync(response, expectedCode);
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

    private async Task<bool> GetIsActiveAsync(string id)
    {
        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/investors/{id}");

        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("isActive").GetBoolean();
    }

    private static async Task AssertProblemCodeAsync(
        HttpResponseMessage response,
        string expectedCode)
    {
        using JsonDocument document = await ReadJsonAsync(response);
        Assert.Equal(expectedCode, document.RootElement.GetProperty("code").GetString());
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record InvestorRequest(string FullName);
}
