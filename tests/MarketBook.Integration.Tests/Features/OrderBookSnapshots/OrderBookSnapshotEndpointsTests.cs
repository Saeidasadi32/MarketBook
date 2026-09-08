// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.OrderBookSnapshots
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Application.Features.OrderBookSnapshots.Commands.CreateOrderBookSnapshot;
using MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshotById;
using MarketBook.Application.Features.OrderBookSnapshots.Queries.GetOrderBookSnapshots;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.OrderBookSnapshots;

/// <summary>EN: Integration tests for order-book snapshot endpoints. FA: تست‌های Integration مربوط به Endpointهای Snapshot دفتر سفارشات.</summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class OrderBookSnapshotEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;

    /// <summary>EN: Initializes the integration tests. FA: تست‌های Integration را مقداردهی می‌کند.</summary>
    public OrderBookSnapshotEndpointsTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Create_And_Get_Should_Persist_Snapshot_And_Levels()
    {
        await _fixture.ResetOrderBookSnapshotsAsync();
        string listingId = await _fixture.CreateOrderBookSnapshotListingAsync();
        CreateOrderBookSnapshotRequest request = CreateRequest(listingId, 1);

        HttpResponseMessage create = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/order-book-snapshots",
            request);

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        JsonElement created = await create.Content.ReadFromJsonAsync<JsonElement>();
        string id = created.GetProperty("id").GetString()!;

        GetOrderBookSnapshotByIdResponse? item =
            await _fixture.Client.GetFromJsonAsync<GetOrderBookSnapshotByIdResponse>(
                $"/api/v1/order-book-snapshots/{id}");

        Assert.NotNull(item);
        Assert.Equal(1L, item.SequenceNumber);
        Assert.Equal(2, item.Levels.Count);
        Assert.Equal(105.20m, item.Levels[0].BidPrice);
        Assert.Equal(105.30m, item.Levels[0].AskPrice);
        Assert.Equal(105.10m, item.Levels[1].BidPrice);
        Assert.Equal(105.40m, item.Levels[1].AskPrice);
    }

    [Fact]
    public async Task Duplicate_Sequence_Should_Return_Conflict()
    {
        await _fixture.ResetOrderBookSnapshotsAsync();
        string listingId = await _fixture.CreateOrderBookSnapshotListingAsync();
        CreateOrderBookSnapshotRequest request = CreateRequest(listingId, 1);

        HttpResponseMessage first = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/order-book-snapshots",
            request);
        HttpResponseMessage second = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/order-book-snapshots",
            request);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task NonContiguous_Levels_Should_Return_BadRequest()
    {
        await _fixture.ResetOrderBookSnapshotsAsync();
        string listingId = await _fixture.CreateOrderBookSnapshotListingAsync();

        IReadOnlyList<CreateOrderBookSnapshotLevelRequest> levels =
        [
            CreateLevel(1, 105.20m, 105.30m),
            CreateLevel(3, 105.10m, 105.40m)
        ];

        HttpResponseMessage response = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/order-book-snapshots",
            CreateRequest(listingId, 1, levels: levels));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Missing_Listing_Should_Return_NotFound()
    {
        await _fixture.ResetOrderBookSnapshotsAsync();

        HttpResponseMessage response = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/order-book-snapshots",
            CreateRequest("01ARZ3NDEKTSV4RRFFQ69G5FAV", 1));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_Should_Order_By_CaptureTime_Then_Sequence()
    {
        await _fixture.ResetOrderBookSnapshotsAsync();
        string listingId = await _fixture.CreateOrderBookSnapshotListingAsync();

        await _fixture.Client.PostAsJsonAsync(
            "/api/v1/order-book-snapshots",
            CreateRequest(listingId, 2, 10, 16, 0));

        await _fixture.Client.PostAsJsonAsync(
            "/api/v1/order-book-snapshots",
            CreateRequest(listingId, 1, 10, 15, 0));

        GetOrderBookSnapshotsResponse? page =
            await _fixture.Client.GetFromJsonAsync<GetOrderBookSnapshotsResponse>(
                $"/api/v1/order-book-snapshots?listingId={listingId}&tradingDate=2026-09-08&page=1&pageSize=20");

        Assert.NotNull(page);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(1L, page.Items[0].SequenceNumber);
        Assert.Equal(2L, page.Items[1].SequenceNumber);
    }

    [Fact]
    public async Task Pagination_Should_Normalize_Request()
    {
        await _fixture.ResetOrderBookSnapshotsAsync();
        string listingId = await _fixture.CreateOrderBookSnapshotListingAsync();

        await _fixture.Client.PostAsJsonAsync(
            "/api/v1/order-book-snapshots",
            CreateRequest(listingId, 1));

        GetOrderBookSnapshotsResponse? page =
            await _fixture.Client.GetFromJsonAsync<GetOrderBookSnapshotsResponse>(
                $"/api/v1/order-book-snapshots?listingId={listingId}&tradingDate=2026-09-08&page=0&pageSize=500");

        Assert.NotNull(page);
        Assert.Equal(1, page.Page);
        Assert.Equal(100, page.PageSize);
        Assert.Equal(1, page.TotalCount);
    }

    private static CreateOrderBookSnapshotRequest CreateRequest(
        string listingId,
        long sequenceNumber,
        int hour = 10,
        int minute = 15,
        int second = 30,
        IReadOnlyList<CreateOrderBookSnapshotLevelRequest>? levels = null)
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
            levels ??
            [
                CreateLevel(1, 105.20m, 105.30m),
                CreateLevel(2, 105.10m, 105.40m)
            ]);

    private static CreateOrderBookSnapshotLevelRequest CreateLevel(
        int level,
        decimal bidPrice,
        decimal askPrice)
        => new(
            level,
            bidPrice,
            2_500L,
            4,
            askPrice,
            3_000L,
            5);
}
