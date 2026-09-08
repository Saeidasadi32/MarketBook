// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.DailyTradeStatistics
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Application.Features.DailyTradeStatistics.Commands.CreateDailyTradeStatistics;
using MarketBook.Application.Features.DailyTradeStatistics.Commands.UpdateDailyTradeStatistics;
using MarketBook.Application.Features.DailyTradeStatistics.Queries.GetAllDailyTradeStatistics;
using MarketBook.Application.Features.DailyTradeStatistics.Queries.GetDailyTradeStatisticsById;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.DailyTradeStatistics;

/// <summary>
/// EN: Integration tests for daily trade-statistics endpoints.
/// FA: تست‌های Integration مربوط به Endpointهای آمار معاملات روزانه.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class DailyTradeStatisticsEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;

    /// <summary>
    /// EN: Initializes the integration tests.
    /// FA: تست‌های Integration را مقداردهی می‌کند.
    /// </summary>
    public DailyTradeStatisticsEndpointsTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Create_And_Get_Should_Persist_Statistics()
    {
        await _fixture.ResetDailyTradeStatisticsAsync();
        string listingId = await _fixture.CreateDailyTradeStatisticsListingAsync();

        CreateDailyTradeStatisticsRequest request = CreateRequest(listingId);
        HttpResponseMessage create = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/daily-trade-statistics",
            request);

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        JsonElement created = await create.Content.ReadFromJsonAsync<JsonElement>();
        string id = created.GetProperty("id").GetString()!;

        GetDailyTradeStatisticsByIdResponse? item =
            await _fixture.Client.GetFromJsonAsync<GetDailyTradeStatisticsByIdResponse>(
                $"/api/v1/daily-trade-statistics/{id}");

        Assert.NotNull(item);
        Assert.Equal(1_250_000L, item.Volume);
        Assert.Equal(842, item.TradeCount);
        Assert.Equal(105.25m, item.AveragePrice);
        Assert.Equal(131_562_500m, item.TradeValue);
        Assert.True(item.AverageTradeSize > 0m);
    }

    [Fact]
    public async Task Duplicate_Listing_Date_Should_Return_Conflict()
    {
        await _fixture.ResetDailyTradeStatisticsAsync();
        string listingId = await _fixture.CreateDailyTradeStatisticsListingAsync();
        CreateDailyTradeStatisticsRequest request = CreateRequest(listingId);

        HttpResponseMessage first = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/daily-trade-statistics",
            request);
        HttpResponseMessage second = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/daily-trade-statistics",
            request);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Negative_Statistics_Should_Return_BadRequest()
    {
        await _fixture.ResetDailyTradeStatisticsAsync();
        string listingId = await _fixture.CreateDailyTradeStatisticsListingAsync();

        CreateDailyTradeStatisticsRequest request = new(
            listingId,
            new DateOnly(2026, 9, 8),
            -1,
            10,
            100m,
            1_000m,
            10_000m);

        HttpResponseMessage response = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/daily-trade-statistics",
            request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Missing_Listing_Should_Return_NotFound()
    {
        await _fixture.ResetDailyTradeStatisticsAsync();

        CreateDailyTradeStatisticsRequest request = CreateRequest(
            "01ARZ3NDEKTSV4RRFFQ69G5FAV");

        HttpResponseMessage response = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/daily-trade-statistics",
            request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Should_Persist_New_Statistics()
    {
        await _fixture.ResetDailyTradeStatisticsAsync();
        string listingId = await _fixture.CreateDailyTradeStatisticsListingAsync();

        HttpResponseMessage create = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/daily-trade-statistics",
            CreateRequest(listingId));

        JsonElement created = await create.Content.ReadFromJsonAsync<JsonElement>();
        string id = created.GetProperty("id").GetString()!;

        UpdateDailyTradeStatisticsRequest update = new(
            2_000_000L,
            1_000,
            108.5m,
            217_000_000m,
            55_000_000_000m);

        HttpResponseMessage updateResponse = await _fixture.Client.PutAsJsonAsync(
            $"/api/v1/daily-trade-statistics/{id}",
            update);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        GetDailyTradeStatisticsByIdResponse? item =
            await _fixture.Client.GetFromJsonAsync<GetDailyTradeStatisticsByIdResponse>(
                $"/api/v1/daily-trade-statistics/{id}");

        Assert.NotNull(item);
        Assert.Equal(2_000_000L, item.Volume);
        Assert.Equal(1_000, item.TradeCount);
        Assert.Equal(108.5m, item.AveragePrice);
    }

    [Fact]
    public async Task Pagination_Should_Normalize_Request()
    {
        await _fixture.ResetDailyTradeStatisticsAsync();
        string listingId = await _fixture.CreateDailyTradeStatisticsListingAsync();

        await _fixture.Client.PostAsJsonAsync(
            "/api/v1/daily-trade-statistics",
            CreateRequest(listingId));

        GetAllDailyTradeStatisticsResponse? page =
            await _fixture.Client.GetFromJsonAsync<GetAllDailyTradeStatisticsResponse>(
                "/api/v1/daily-trade-statistics?page=0&pageSize=500");

        Assert.NotNull(page);
        Assert.Equal(1, page.Page);
        Assert.Equal(100, page.PageSize);
        Assert.Equal(1, page.TotalCount);
    }

    private static CreateDailyTradeStatisticsRequest CreateRequest(string listingId)
        => new(
            listingId,
            new DateOnly(2026, 9, 8),
            1_250_000L,
            842,
            105.25m,
            131_562_500m,
            50_000_000_000m);
}
