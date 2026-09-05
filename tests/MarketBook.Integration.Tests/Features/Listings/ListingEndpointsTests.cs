// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.Listings
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.Listings;

/// <summary>
/// EN: Verifies Listing HTTP endpoints through the real API, EF Core, and SQL Server pipeline.
/// FA: Endpointهای Listing را از مسیر واقعی API، EF Core و SQL Server بررسی می‌کند.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class ListingEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes the Listing endpoint test suite.
    /// FA: مجموعه تست Endpointهای Listing را مقداردهی می‌کند.
    /// </summary>
    public ListingEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies creating and retrieving a listing with all relationship identifiers.
    /// FA: ایجاد و دریافت Listing را همراه با همه شناسه‌های رابطه‌ای بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Create_And_GetById_Should_Succeed()
    {
        await _fixture.ResetListingsAsync();
        ListingTestSeed seed = await _fixture.CreateListingSeedAsync();

        string id =
            await CreateListingAsync(
                seed,
                "AAPL",
                0.01m,
                2);

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/listings/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root = document.RootElement;

        Assert.Equal(id, root.GetProperty("id").GetString());
        Assert.Equal(seed.InstrumentId, root.GetProperty("instrumentId").GetString());
        Assert.Equal(seed.VenueId, root.GetProperty("venueId").GetString());
        Assert.Equal(seed.QuoteCurrencyId, root.GetProperty("quoteCurrencyId").GetString());
        Assert.Equal("AAPL", root.GetProperty("tradingSymbol").GetString());
        Assert.Equal(0.01m, root.GetProperty("tickSize").GetDecimal());
        Assert.Equal(2, root.GetProperty("pricePrecision").GetInt32());
        Assert.False(root.GetProperty("isPrimary").GetBoolean());
        Assert.True(root.GetProperty("isActive").GetBoolean());
    }

    /// <summary>
    /// EN: Verifies trading symbols are unique inside a venue.
    /// FA: یکتایی نماد معاملاتی را در محدوده یک Venue بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Create_DuplicateSymbol_InSameVenue_Should_Return_Conflict()
    {
        await _fixture.ResetListingsAsync();
        ListingTestSeed seed = await _fixture.CreateListingSeedAsync();

        await CreateListingAsync(seed, "DUPL", 0.01m, 2);

        using HttpResponseMessage response =
            await PostListingAsync(seed, "DUPL", 0.01m, 2);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "Listing.DuplicateTradingSymbol");
    }

    /// <summary>
    /// EN: Verifies create-request validation for symbol, tick size, and price precision.
    /// FA: اعتبارسنجی Symbol، TickSize و PricePrecision در درخواست ایجاد را بررسی می‌کند.
    /// </summary>
    [Theory]
    [InlineData("BAD SYMBOL", 0.01, 2, "Listing.InvalidTradingSymbol")]
    [InlineData("VALID", 0.0, 2, "Listing.InvalidTickSize")]
    [InlineData("VALID", -0.01, 2, "Listing.InvalidTickSize")]
    [InlineData("VALID", 0.01, -1, "Listing.InvalidPricePrecision")]
    [InlineData("VALID", 0.01, 29, "Listing.InvalidPricePrecision")]
    public async Task Create_InvalidRequest_Should_Return_BadRequest(
        string symbol,
        double tickSize,
        int pricePrecision,
        string expectedCode)
    {
        await _fixture.ResetListingsAsync();
        ListingTestSeed seed = await _fixture.CreateListingSeedAsync();

        using HttpResponseMessage response =
            await PostListingAsync(
                seed,
                symbol,
                Convert.ToDecimal(tickSize),
                pricePrecision);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, expectedCode);
    }

    /// <summary>
    /// EN: Verifies a valid but missing instrument is rejected.
    /// FA: رد شدن Instrument معتبر از نظر شناسه ولی ناموجود را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Create_MissingInstrument_Should_Return_NotFound()
    {
        await _fixture.ResetListingsAsync();
        ListingTestSeed seed = await _fixture.CreateListingSeedAsync();

        ListingRequest request = new(
            "01ARZ3NDEKTSV4RRFFQ69G5FAV",
            seed.VenueId,
            seed.QuoteCurrencyId,
            "MISS",
            0.01m,
            2);

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/listings",
                request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "Listing.InstrumentNotFound");
    }

    /// <summary>
    /// EN: Verifies updating mutable fields while InstrumentId and VenueId remain unchanged.
    /// FA: به‌روزرسانی فیلدهای قابل تغییر را در حالی بررسی می‌کند که InstrumentId و VenueId ثابت می‌مانند.
    /// </summary>
    [Fact]
    public async Task Update_Should_Persist_Mutable_Fields()
    {
        await _fixture.ResetListingsAsync();
        ListingTestSeed seed = await _fixture.CreateListingSeedAsync();

        string id =
            await CreateListingAsync(seed, "BEFORE", 0.01m, 2);

        UpdateListingRequest request = new(
            seed.QuoteCurrencyId,
            "AFTER",
            0.0001m,
            4);

        using HttpResponseMessage updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/v1/listings/{id}",
                request);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        using HttpResponseMessage getResponse =
            await _client.GetAsync($"/api/v1/listings/{id}");

        using JsonDocument document =
            await ReadJsonAsync(getResponse);

        JsonElement root = document.RootElement;

        Assert.Equal(seed.InstrumentId, root.GetProperty("instrumentId").GetString());
        Assert.Equal(seed.VenueId, root.GetProperty("venueId").GetString());
        Assert.Equal("AFTER", root.GetProperty("tradingSymbol").GetString());
        Assert.Equal(0.0001m, root.GetProperty("tickSize").GetDecimal());
        Assert.Equal(4, root.GetProperty("pricePrecision").GetInt32());
    }

    /// <summary>
    /// EN: Verifies activate/deactivate operations are persisted and idempotent.
    /// FA: Persist شدن و idempotent بودن فعال‌سازی و غیرفعال‌سازی را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Activate_And_Deactivate_Should_Be_Idempotent()
    {
        await _fixture.ResetListingsAsync();
        ListingTestSeed seed = await _fixture.CreateListingSeedAsync();

        string id =
            await CreateListingAsync(seed, "STATE", 0.01m, 2);

        using HttpResponseMessage deactivateResponse =
            await _client.PatchAsync($"/api/v1/listings/{id}/deactivate", null);

        Assert.Equal(HttpStatusCode.OK, deactivateResponse.StatusCode);

        using HttpResponseMessage secondDeactivateResponse =
            await _client.PatchAsync($"/api/v1/listings/{id}/deactivate", null);

        Assert.Equal(HttpStatusCode.OK, secondDeactivateResponse.StatusCode);
        Assert.False(await GetBooleanAsync(id, "isActive"));

        using HttpResponseMessage activateResponse =
            await _client.PatchAsync($"/api/v1/listings/{id}/activate", null);

        Assert.Equal(HttpStatusCode.OK, activateResponse.StatusCode);

        using HttpResponseMessage secondActivateResponse =
            await _client.PatchAsync($"/api/v1/listings/{id}/activate", null);

        Assert.Equal(HttpStatusCode.OK, secondActivateResponse.StatusCode);
        Assert.True(await GetBooleanAsync(id, "isActive"));
    }

    /// <summary>
    /// EN: Verifies switching the single primary listing for one instrument.
    /// FA: جابه‌جایی تنها Listing اصلی یک Instrument را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task MakePrimary_Should_Move_Primary_Flag()
    {
        await _fixture.ResetListingsAsync();
        ListingTestSeed seed = await _fixture.CreateListingSeedAsync();

        string firstId =
            await CreateListingAsync(seed, "PRI1", 0.01m, 2);

        string secondId =
            await CreateListingAsync(seed, "PRI2", 0.01m, 2);

        using HttpResponseMessage firstResponse =
            await _client.PatchAsync(
                $"/api/v1/listings/{firstId}/make-primary",
                null);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.True(await GetBooleanAsync(firstId, "isPrimary"));

        using HttpResponseMessage secondResponse =
            await _client.PatchAsync(
                $"/api/v1/listings/{secondId}/make-primary",
                null);

        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        Assert.False(await GetBooleanAsync(firstId, "isPrimary"));
        Assert.True(await GetBooleanAsync(secondId, "isPrimary"));

        using HttpResponseMessage idempotentResponse =
            await _client.PatchAsync(
                $"/api/v1/listings/{secondId}/make-primary",
                null);

        Assert.Equal(HttpStatusCode.OK, idempotentResponse.StatusCode);
    }

    /// <summary>
    /// EN: Verifies paged retrieval and normalization boundaries.
    /// FA: دریافت صفحه‌بندی‌شده و مرزهای Normalization را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task GetAll_Should_Page_And_Normalize_Request()
    {
        await _fixture.ResetListingsAsync();
        ListingTestSeed seed = await _fixture.CreateListingSeedAsync();

        await CreateListingAsync(seed, "AAA", 0.01m, 2);
        await CreateListingAsync(seed, "BBB", 0.01m, 2);
        await CreateListingAsync(seed, "CCC", 0.01m, 2);

        using HttpResponseMessage pagedResponse =
            await _client.GetAsync("/api/v1/listings?page=2&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, pagedResponse.StatusCode);

        using JsonDocument pagedDocument =
            await ReadJsonAsync(pagedResponse);

        JsonElement pagedRoot = pagedDocument.RootElement;

        Assert.Equal(2, pagedRoot.GetProperty("page").GetInt32());
        Assert.Equal(1, pagedRoot.GetProperty("pageSize").GetInt32());
        Assert.Equal(3, pagedRoot.GetProperty("totalCount").GetInt32());
        Assert.Equal(3, pagedRoot.GetProperty("totalPages").GetInt32());
        Assert.Single(pagedRoot.GetProperty("items").EnumerateArray());

        using HttpResponseMessage normalizedResponse =
            await _client.GetAsync("/api/v1/listings?page=-10&pageSize=-5");

        Assert.Equal(HttpStatusCode.OK, normalizedResponse.StatusCode);

        using JsonDocument normalizedDocument =
            await ReadJsonAsync(normalizedResponse);

        JsonElement normalizedRoot = normalizedDocument.RootElement;

        Assert.Equal(1, normalizedRoot.GetProperty("page").GetInt32());
        Assert.Equal(20, normalizedRoot.GetProperty("pageSize").GetInt32());
    }

    /// <summary>
    /// EN: Verifies invalid and missing listing identifiers.
    /// FA: شناسه نامعتبر و شناسه ناموجود Listing را بررسی می‌کند.
    /// </summary>
    [Theory]
    [InlineData("abc", HttpStatusCode.BadRequest, "Listing.InvalidId")]
    [InlineData("01ARZ3NDEKTSV4RRFFQ69G5FAV", HttpStatusCode.NotFound, "Listing.NotFound")]
    public async Task GetById_InvalidOrMissingId_Should_Return_ExpectedError(
        string id,
        HttpStatusCode expectedStatusCode,
        string expectedCode)
    {
        await _fixture.ResetListingsAsync();

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/listings/{id}");

        Assert.Equal(expectedStatusCode, response.StatusCode);
        await AssertProblemCodeAsync(response, expectedCode);
    }

    private async Task<string> CreateListingAsync(
        ListingTestSeed seed,
        string symbol,
        decimal tickSize,
        int pricePrecision)
    {
        using HttpResponseMessage response =
            await PostListingAsync(
                seed,
                symbol,
                tickSize,
                pricePrecision);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        string? id =
            document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));

        return id!;
    }

    private Task<HttpResponseMessage> PostListingAsync(
        ListingTestSeed seed,
        string symbol,
        decimal tickSize,
        int pricePrecision)
    {
        ListingRequest request = new(
            seed.InstrumentId,
            seed.VenueId,
            seed.QuoteCurrencyId,
            symbol,
            tickSize,
            pricePrecision);

        return _client.PostAsJsonAsync(
            "/api/v1/listings",
            request);
    }

    private async Task<bool> GetBooleanAsync(
        string id,
        string propertyName)
    {
        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/listings/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        return document.RootElement
            .GetProperty(propertyName)
            .GetBoolean();
    }

    private static async Task AssertProblemCodeAsync(
        HttpResponseMessage response,
        string expectedCode)
    {
        using JsonDocument document =
            await ReadJsonAsync(response);

        Assert.Equal(
            expectedCode,
            document.RootElement.GetProperty("code").GetString());
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response)
    {
        return await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());
    }

    private sealed record ListingRequest(
        string InstrumentId,
        string VenueId,
        string QuoteCurrencyId,
        string TradingSymbol,
        decimal TickSize,
        int PricePrecision);

    private sealed record UpdateListingRequest(
        string QuoteCurrencyId,
        string TradingSymbol,
        decimal TickSize,
        int PricePrecision);
}
