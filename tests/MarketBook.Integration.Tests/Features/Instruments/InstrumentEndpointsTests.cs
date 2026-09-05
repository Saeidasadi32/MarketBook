// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.Instruments
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.Instruments;

/// <summary>
/// EN: Verifies Instrument HTTP endpoints against the real SQL Server persistence pipeline.
/// FA: Endpointهای HTTP ابزار مالی را در برابر مسیر واقعی Persistence مبتنی بر SQL Server بررسی می‌کند.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class InstrumentEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes a new Instrument endpoint test suite.
    /// FA: مجموعه تست جدید Endpointهای ابزار مالی را ایجاد می‌کند.
    /// </summary>
    public InstrumentEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies creating and retrieving an instrument.
    /// FA: ایجاد و دریافت ابزار مالی را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Create_And_GetById_Should_Succeed()
    {
        await _fixture.ResetInstrumentsAsync();

        string id =
            await CreateInstrumentAsync(
                "Apple Inc.",
                "Equity",
                1,
                "Equity",
                "US0378331005");

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/instruments/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root = document.RootElement;

        Assert.Equal(id, root.GetProperty("id").GetString());
        Assert.Equal("Apple Inc.", root.GetProperty("name").GetString());
        Assert.Equal("Equity", root.GetProperty("assetClass").GetString());
        Assert.Equal(1, root.GetProperty("type").GetInt32());
        Assert.Equal("Equity", root.GetProperty("category").GetString());
        Assert.Equal("US0378331005", root.GetProperty("isin").GetString());
        Assert.True(root.GetProperty("isActive").GetBoolean());
    }

    /// <summary>
    /// EN: Verifies duplicate ISIN values are rejected.
    /// FA: رد شدن ISIN تکراری را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Create_DuplicateIsin_Should_Return_Conflict()
    {
        await _fixture.ResetInstrumentsAsync();

        await CreateInstrumentAsync(
            "Microsoft Corporation",
            "Equity",
            1,
            "Equity",
            "US5949181045");

        using HttpResponseMessage response =
            await PostInstrumentAsync(
                "Duplicate Instrument",
                "Equity",
                1,
                "Equity",
                "US5949181045");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        await AssertProblemCodeAsync(
            response,
            "Instrument.DuplicateIsin");
    }

    /// <summary>
    /// EN: Verifies create validation.
    /// FA: اعتبارسنجی درخواست ایجاد را بررسی می‌کند.
    /// </summary>
    [Theory]
    [InlineData("", "Equity", 1, "Equity", null, "Instrument.InvalidName")]
    [InlineData("Test", "", 1, "Equity", null, "Instrument.InvalidAssetClass")]
    [InlineData("Test", "Equity", 999, "Equity", null, "Instrument.InvalidType")]
    [InlineData("Test", "Equity", 1, "", null, "Instrument.InvalidCategory")]
    [InlineData("Test", "Equity", 1, "Equity", "BAD", "Instrument.InvalidIsin")]
    public async Task Create_InvalidRequest_Should_Return_BadRequest(
        string name,
        string assetClass,
        int type,
        string category,
        string? isin,
        string expectedErrorCode)
    {
        await _fixture.ResetInstrumentsAsync();

        using HttpResponseMessage response =
            await PostInstrumentAsync(
                name,
                assetClass,
                type,
                category,
                isin);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await AssertProblemCodeAsync(
            response,
            expectedErrorCode);
    }

    /// <summary>
    /// EN: Verifies updating and persisting instrument attributes.
    /// FA: به‌روزرسانی و Persist شدن ویژگی‌های ابزار مالی را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Update_Should_Persist_Changes_And_Remove_Isin()
    {
        await _fixture.ResetInstrumentsAsync();

        string id =
            await CreateInstrumentAsync(
                "Before Update",
                "Equity",
                1,
                "Equity",
                "US0378331005");

        InstrumentRequest request = new(
            "After Update",
            "Derivative",
            2,
            "Derivative",
            null);

        using HttpResponseMessage updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/v1/instruments/{id}",
                request);

        Assert.Equal(
            HttpStatusCode.OK,
            updateResponse.StatusCode);

        using HttpResponseMessage getResponse =
            await _client.GetAsync(
                $"/api/v1/instruments/{id}");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(getResponse);

        JsonElement root = document.RootElement;

        Assert.Equal("After Update", root.GetProperty("name").GetString());
        Assert.Equal("Derivative", root.GetProperty("assetClass").GetString());
        Assert.Equal(2, root.GetProperty("type").GetInt32());
        Assert.Equal("Derivative", root.GetProperty("category").GetString());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("isin").ValueKind);
    }

    /// <summary>
    /// EN: Verifies activate and deactivate operations are idempotent.
    /// FA: idempotent بودن فعال‌سازی و غیرفعال‌سازی را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Activate_And_Deactivate_Should_Be_Idempotent()
    {
        await _fixture.ResetInstrumentsAsync();

        string id =
            await CreateInstrumentAsync(
                "State Instrument",
                "Equity",
                1,
                "Equity",
                null);

        using HttpResponseMessage deactivateResponse =
            await _client.PatchAsync(
                $"/api/v1/instruments/{id}/deactivate",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
            deactivateResponse.StatusCode);

        using HttpResponseMessage secondDeactivateResponse =
            await _client.PatchAsync(
                $"/api/v1/instruments/{id}/deactivate",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
            secondDeactivateResponse.StatusCode);

        Assert.False(await GetIsActiveAsync(id));

        using HttpResponseMessage activateResponse =
            await _client.PatchAsync(
                $"/api/v1/instruments/{id}/activate",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
            activateResponse.StatusCode);

        using HttpResponseMessage secondActivateResponse =
            await _client.PatchAsync(
                $"/api/v1/instruments/{id}/activate",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
            secondActivateResponse.StatusCode);

        Assert.True(await GetIsActiveAsync(id));
    }

    /// <summary>
    /// EN: Verifies pagination and request normalization.
    /// FA: صفحه‌بندی و نرمال‌سازی درخواست را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task GetAll_Should_Page_And_Normalize_Request()
    {
        await _fixture.ResetInstrumentsAsync();

        await CreateInstrumentAsync("Instrument A", "Equity", 1, "Equity", null);
        await CreateInstrumentAsync("Instrument B", "Equity", 1, "Equity", null);
        await CreateInstrumentAsync("Instrument C", "Equity", 1, "Equity", null);

        using HttpResponseMessage pagedResponse =
            await _client.GetAsync(
                "/api/v1/instruments?page=2&pageSize=1");

        Assert.Equal(
            HttpStatusCode.OK,
            pagedResponse.StatusCode);

        using JsonDocument pagedDocument =
            await ReadJsonAsync(pagedResponse);

        JsonElement pagedRoot = pagedDocument.RootElement;

        Assert.Equal(2, pagedRoot.GetProperty("page").GetInt32());
        Assert.Equal(1, pagedRoot.GetProperty("pageSize").GetInt32());
        Assert.Equal(3, pagedRoot.GetProperty("totalCount").GetInt32());
        Assert.Equal(3, pagedRoot.GetProperty("totalPages").GetInt32());
        Assert.Single(pagedRoot.GetProperty("items").EnumerateArray());

        using HttpResponseMessage normalizedResponse =
            await _client.GetAsync(
                "/api/v1/instruments?page=-10&pageSize=-5");

        Assert.Equal(
            HttpStatusCode.OK,
            normalizedResponse.StatusCode);

        using JsonDocument normalizedDocument =
            await ReadJsonAsync(normalizedResponse);

        JsonElement normalizedRoot = normalizedDocument.RootElement;

        Assert.Equal(1, normalizedRoot.GetProperty("page").GetInt32());
        Assert.Equal(20, normalizedRoot.GetProperty("pageSize").GetInt32());
    }

    /// <summary>
    /// EN: Verifies invalid and missing identifiers.
    /// FA: شناسه نامعتبر و شناسه ناموجود را بررسی می‌کند.
    /// </summary>
    [Theory]
    [InlineData("abc", HttpStatusCode.BadRequest, "Instrument.InvalidId")]
    [InlineData("01ARZ3NDEKTSV4RRFFQ69G5FAV", HttpStatusCode.NotFound, "Instrument.NotFound")]
    public async Task GetById_InvalidOrMissingId_Should_Return_ExpectedError(
        string id,
        HttpStatusCode expectedStatusCode,
        string expectedErrorCode)
    {
        await _fixture.ResetInstrumentsAsync();

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/instruments/{id}");

        Assert.Equal(
            expectedStatusCode,
            response.StatusCode);

        await AssertProblemCodeAsync(
            response,
            expectedErrorCode);
    }

    private async Task<string> CreateInstrumentAsync(
        string name,
        string assetClass,
        int type,
        string category,
        string? isin)
    {
        using HttpResponseMessage response =
            await PostInstrumentAsync(
                name,
                assetClass,
                type,
                category,
                isin);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        string? id =
            document.RootElement
                .GetProperty("id")
                .GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));

        return id!;
    }

    private Task<HttpResponseMessage> PostInstrumentAsync(
        string name,
        string assetClass,
        int type,
        string category,
        string? isin)
    {
        InstrumentRequest request = new(
            name,
            assetClass,
            type,
            category,
            isin);

        return _client.PostAsJsonAsync(
            "/api/v1/instruments",
            request);
    }

    private async Task<bool> GetIsActiveAsync(string id)
    {
        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/instruments/{id}");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        return document.RootElement
            .GetProperty("isActive")
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
            document.RootElement
                .GetProperty("code")
                .GetString());
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response)
    {
        return await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());
    }

    private sealed record InstrumentRequest(
        string Name,
        string AssetClass,
        int Type,
        string Category,
        string? Isin);
}
