// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.IntradayPriceTicks
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Application.Features.IntradayPriceTicks.Commands.CreateIntradayPriceTick;
using MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTickById;
using MarketBook.Application.Features.IntradayPriceTicks.Queries.GetIntradayPriceTicks;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.IntradayPriceTicks;

/// <summary>
/// EN: Integration tests for intraday price-tick endpoints.
/// FA: تست‌های Integration مربوط به Endpointهای Tick قیمت درون‌روزی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class IntradayPriceTickEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;

    /// <summary>
    /// EN: Initializes the integration tests.
    /// FA: تست‌های Integration را مقداردهی می‌کند.
    /// </summary>
    public IntradayPriceTickEndpointsTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Create_And_Get_Should_Persist_Tick()
    {
        await _fixture.ResetIntradayPriceTicksAsync();
        string listingId = await _fixture.CreateIntradayPriceTickListingAsync();

        CreateIntradayPriceTickRequest request = CreateRequest(listingId, 1);

        HttpResponseMessage create = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/intraday-price-ticks",
            request);

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        JsonElement created = await create.Content.ReadFromJsonAsync<JsonElement>();
        string id = created.GetProperty("id").GetString()!;

        GetIntradayPriceTickByIdResponse? item =
            await _fixture.Client.GetFromJsonAsync<GetIntradayPriceTickByIdResponse>(
                $"/api/v1/intraday-price-ticks/{id}");

        Assert.NotNull(item);
        Assert.Equal(105.25m, item.Price);
        Assert.Equal(2_500L, item.Volume);
        Assert.Equal(263_125m, item.TradeValue);
        Assert.Equal(1L, item.SequenceNumber);
    }

    [Fact]
    public async Task Duplicate_Sequence_Should_Return_Conflict()
    {
        await _fixture.ResetIntradayPriceTicksAsync();
        string listingId = await _fixture.CreateIntradayPriceTickListingAsync();
        CreateIntradayPriceTickRequest request = CreateRequest(listingId, 1);

        HttpResponseMessage first = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/intraday-price-ticks",
            request);
        HttpResponseMessage second = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/intraday-price-ticks",
            request);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task NonPositive_Price_Should_Return_BadRequest()
    {
        await _fixture.ResetIntradayPriceTicksAsync();
        string listingId = await _fixture.CreateIntradayPriceTickListingAsync();

        CreateIntradayPriceTickRequest request = new(
            listingId,
            new DateOnly(2026, 9, 8),
            new DateTimeOffset(2026, 9, 8, 10, 15, 30, TimeSpan.FromHours(3.5)),
            1,
            0m,
            100L);

        HttpResponseMessage response = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/intraday-price-ticks",
            request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Missing_Listing_Should_Return_NotFound()
    {
        await _fixture.ResetIntradayPriceTicksAsync();

        HttpResponseMessage response = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/intraday-price-ticks",
            CreateRequest("01ARZ3NDEKTSV4RRFFQ69G5FAV", 1));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_Should_Order_By_Time_Then_Sequence()
    {
        await _fixture.ResetIntradayPriceTicksAsync();
        string listingId = await _fixture.CreateIntradayPriceTickListingAsync();

        await _fixture.Client.PostAsJsonAsync(
            "/api/v1/intraday-price-ticks",
            CreateRequest(listingId, 2, 10, 16, 0));

        await _fixture.Client.PostAsJsonAsync(
            "/api/v1/intraday-price-ticks",
            CreateRequest(listingId, 1, 10, 15, 0));

        GetIntradayPriceTicksResponse? page =
            await _fixture.Client.GetFromJsonAsync<GetIntradayPriceTicksResponse>(
                $"/api/v1/intraday-price-ticks?listingId={listingId}&tradingDate=2026-09-08&page=1&pageSize=20");

        Assert.NotNull(page);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(1L, page.Items[0].SequenceNumber);
        Assert.Equal(2L, page.Items[1].SequenceNumber);
    }

    [Fact]
    public async Task Pagination_Should_Normalize_Request()
    {
        await _fixture.ResetIntradayPriceTicksAsync();
        string listingId = await _fixture.CreateIntradayPriceTickListingAsync();

        await _fixture.Client.PostAsJsonAsync(
            "/api/v1/intraday-price-ticks",
            CreateRequest(listingId, 1));

        GetIntradayPriceTicksResponse? page =
            await _fixture.Client.GetFromJsonAsync<GetIntradayPriceTicksResponse>(
                $"/api/v1/intraday-price-ticks?listingId={listingId}&tradingDate=2026-09-08&page=0&pageSize=500");

        Assert.NotNull(page);
        Assert.Equal(1, page.Page);
        Assert.Equal(100, page.PageSize);
        Assert.Equal(1, page.TotalCount);
    }

    private static CreateIntradayPriceTickRequest CreateRequest(
        string listingId,
        long sequenceNumber,
        int hour = 10,
        int minute = 15,
        int second = 30)
        => new(
            listingId,
            new DateOnly(2026, 9, 8),
            new DateTimeOffset(
                2026,
                9,
                8,
                hour,
                minute,
                second,
                TimeSpan.FromHours(3.5)),
            sequenceNumber,
            105.25m,
            2_500L);
}
