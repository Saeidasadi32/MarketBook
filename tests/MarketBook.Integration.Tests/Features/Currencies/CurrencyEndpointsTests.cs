// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.Currencies
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.Currencies;

/// <summary>
/// EN: Verifies Currency HTTP endpoints against the real SQL Server persistence pipeline.
/// FA: Endpointهای HTTP ارز را در برابر مسیر واقعی Persistence مبتنی بر SQL Server بررسی می‌کند.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class CurrencyEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes a new Currency endpoint test suite.
    /// FA: مجموعه تست جدید Endpointهای ارز را ایجاد می‌کند.
    /// </summary>
    /// <param name="fixture">
    /// EN: Shared integration-test fixture.
    /// FA: Fixture مشترک تست Integration.
    /// </param>
    public CurrencyEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies creating and retrieving a currency through the API.
    /// FA: ایجاد و دریافت ارز از طریق API را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Create_And_GetById_Should_Succeed()
    {
        await _fixture.ResetCurrenciesAsync();

        string id = await CreateCurrencyAsync("XAA", "Test Currency A", 2);

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/currencies/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root = document.RootElement;

        Assert.Equal(id, root.GetProperty("id").GetString());
        Assert.Equal("XAA", root.GetProperty("code").GetString());
        Assert.Equal("Test Currency A", root.GetProperty("name").GetString());
        Assert.Equal(2, root.GetProperty("decimalPlaces").GetInt32());
        Assert.True(root.GetProperty("isActive").GetBoolean());
    }

    /// <summary>
    /// EN: Verifies duplicate currency codes are rejected with HTTP 409.
    /// FA: رد شدن کد تکراری ارز با HTTP 409 را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Create_DuplicateCode_Should_Return_Conflict()
    {
        await _fixture.ResetCurrenciesAsync();

        await CreateCurrencyAsync("XAB", "Test Currency B", 2);

        using HttpResponseMessage response =
            await PostCurrencyAsync("XAB", "Duplicate Currency", 2);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "Currency.DuplicateCode");
    }

    /// <summary>
    /// EN: Verifies create-request validation for code, name, and decimal places.
    /// FA: اعتبارسنجی Code، Name و DecimalPlaces در درخواست ایجاد را بررسی می‌کند.
    /// </summary>
    [Theory]
    [InlineData("US", "Invalid Code", 2, "Currency.InvalidCode")]
    [InlineData("XAC", "", 2, "Currency.InvalidName")]
    [InlineData("XAC", "Invalid Decimal Places", -1, "Currency.InvalidDecimalPlaces")]
    [InlineData("XAC", "Invalid Decimal Places", 19, "Currency.InvalidDecimalPlaces")]
    public async Task Create_InvalidRequest_Should_Return_BadRequest(
        string code,
        string name,
        int decimalPlaces,
        string expectedErrorCode)
    {
        await _fixture.ResetCurrenciesAsync();

        using HttpResponseMessage response =
            await PostCurrencyAsync(code, name, decimalPlaces);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, expectedErrorCode);
    }

    /// <summary>
    /// EN: Verifies updating an existing currency and reading the persisted values.
    /// FA: به‌روزرسانی ارز موجود و خواندن مقادیر Persist شده را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Update_Should_Persist_Changes()
    {
        await _fixture.ResetCurrenciesAsync();

        string id = await CreateCurrencyAsync("XAD", "Before Update", 2);

        CurrencyRequest request = new(
            "XAE",
            "After Update",
            4);

        using HttpResponseMessage updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/v1/currencies/{id}",
                request);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        using HttpResponseMessage getResponse =
            await _client.GetAsync($"/api/v1/currencies/{id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(getResponse);

        JsonElement root = document.RootElement;

        Assert.Equal("XAE", root.GetProperty("code").GetString());
        Assert.Equal("After Update", root.GetProperty("name").GetString());
        Assert.Equal(4, root.GetProperty("decimalPlaces").GetInt32());
    }

    /// <summary>
    /// EN: Verifies deactivate and activate operations are persisted and idempotent.
    /// FA: Persist شدن و idempotent بودن عملیات غیرفعال‌سازی و فعال‌سازی را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Activate_And_Deactivate_Should_Be_Idempotent()
    {
        await _fixture.ResetCurrenciesAsync();

        string id = await CreateCurrencyAsync("XAF", "State Currency", 2);

        using HttpResponseMessage deactivateResponse =
            await _client.PatchAsync(
                $"/api/v1/currencies/{id}/deactivate",
                null);

        Assert.Equal(HttpStatusCode.OK, deactivateResponse.StatusCode);

        using HttpResponseMessage secondDeactivateResponse =
            await _client.PatchAsync(
                $"/api/v1/currencies/{id}/deactivate",
                null);

        Assert.Equal(HttpStatusCode.OK, secondDeactivateResponse.StatusCode);
        Assert.False(await GetIsActiveAsync(id));

        using HttpResponseMessage activateResponse =
            await _client.PatchAsync(
                $"/api/v1/currencies/{id}/activate",
                null);

        Assert.Equal(HttpStatusCode.OK, activateResponse.StatusCode);

        using HttpResponseMessage secondActivateResponse =
            await _client.PatchAsync(
                $"/api/v1/currencies/{id}/activate",
                null);

        Assert.Equal(HttpStatusCode.OK, secondActivateResponse.StatusCode);
        Assert.True(await GetIsActiveAsync(id));
    }

    /// <summary>
    /// EN: Verifies server-side pagination and normalization boundaries.
    /// FA: صفحه‌بندی سمت سرور و مرزهای Normalization آن را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task GetAll_Should_Page_And_Normalize_Request()
    {
        await _fixture.ResetCurrenciesAsync();

        await CreateCurrencyAsync("XAG", "Currency G", 2);
        await CreateCurrencyAsync("XAH", "Currency H", 2);
        await CreateCurrencyAsync("XAI", "Currency I", 2);

        using HttpResponseMessage pagedResponse =
            await _client.GetAsync("/api/v1/currencies?page=2&pageSize=1");

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
            await _client.GetAsync("/api/v1/currencies?page=-10&pageSize=-5");

        Assert.Equal(HttpStatusCode.OK, normalizedResponse.StatusCode);

        using JsonDocument normalizedDocument =
            await ReadJsonAsync(normalizedResponse);

        JsonElement normalizedRoot = normalizedDocument.RootElement;

        Assert.Equal(1, normalizedRoot.GetProperty("page").GetInt32());
        Assert.Equal(20, normalizedRoot.GetProperty("pageSize").GetInt32());
    }

    /// <summary>
    /// EN: Verifies invalid identifiers and valid-but-missing identifiers return the expected API errors.
    /// FA: خطاهای API برای شناسه نامعتبر و شناسه معتبر ولی ناموجود را بررسی می‌کند.
    /// </summary>
    [Theory]
    [InlineData("abc", HttpStatusCode.BadRequest, "Currency.InvalidId")]
    [InlineData("01ARZ3NDEKTSV4RRFFQ69G5FAV", HttpStatusCode.NotFound, "Currency.NotFound")]
    public async Task GetById_InvalidOrMissingId_Should_Return_ExpectedError(
        string id,
        HttpStatusCode expectedStatusCode,
        string expectedErrorCode)
    {
        await _fixture.ResetCurrenciesAsync();

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/currencies/{id}");

        Assert.Equal(expectedStatusCode, response.StatusCode);
        await AssertProblemCodeAsync(response, expectedErrorCode);
    }

    private async Task<string> CreateCurrencyAsync(
        string code,
        string name,
        int decimalPlaces)
    {
        using HttpResponseMessage response =
            await PostCurrencyAsync(code, name, decimalPlaces);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));

        return id!;
    }

    private Task<HttpResponseMessage> PostCurrencyAsync(
        string code,
        string name,
        int decimalPlaces)
    {
        CurrencyRequest request = new(
            code,
            name,
            decimalPlaces);

        return _client.PostAsJsonAsync(
            "/api/v1/currencies",
            request);
    }

    private async Task<bool> GetIsActiveAsync(string id)
    {
        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/currencies/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

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
            document.RootElement.GetProperty("code").GetString());
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response)
    {
        return await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());
    }

    private sealed record CurrencyRequest(
        string Code,
        string Name,
        int DecimalPlaces);
}
