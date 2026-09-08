// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.TradingCalendars
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.TradingCalendars;

/// <summary>
/// EN: Verifies configurable trading-calendar and session behavior through the real SQL Server pipeline.
/// FA: رفتار قابل تنظیم تقویم معاملاتی و Sessionها را از مسیر واقعی SQL Server بررسی می‌کند.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class TradingCalendarEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes the endpoint test suite.
    /// FA: مجموعه تست Endpointها را مقداردهی می‌کند.
    /// </summary>
    public TradingCalendarEndpointsTests(
        IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Proves weekend behavior is configuration-driven rather than hard-coded to Saturday/Sunday.
    /// FA: اثبات می‌کند رفتار آخرهفته از تنظیمات می‌آید و به شنبه/یکشنبه Hard-code نشده است.
    /// </summary>
    [Fact]
    public async Task ConfigurableWeekend_Should_Allow_Saturday_And_Block_Friday()
    {
        await _fixture.ResetTradingCalendarsAsync();
        string marketId =
            await _fixture.CreateTradingCalendarMarketAsync();

        string id =
            await CreateCalendarAsync(
                marketId,
                2026,
                32,
                DefaultSessions(),
                DefaultExceptions());

        using HttpResponseMessage saturdayResponse =
            await _client.GetAsync(
                $"/api/v1/trading-calendars/{id}/days/2026-09-05");

        Assert.Equal(HttpStatusCode.OK, saturdayResponse.StatusCode);

        using JsonDocument saturdayDocument =
            await ReadJsonAsync(saturdayResponse);

        Assert.True(
            saturdayDocument.RootElement
                .GetProperty("isTradingDay")
                .GetBoolean());

        Assert.Equal(
            "09:00:00",
            saturdayDocument.RootElement
                .GetProperty("opensAt")
                .GetString());

        using HttpResponseMessage fridayResponse =
            await _client.GetAsync(
                $"/api/v1/trading-calendars/{id}/days/2026-09-04");

        using JsonDocument fridayDocument =
            await ReadJsonAsync(fridayResponse);

        Assert.False(
            fridayDocument.RootElement
                .GetProperty("isTradingDay")
                .GetBoolean());
    }

    /// <summary>
    /// EN: Verifies holidays and half-days are persisted and evaluated.
    /// FA: Persist و ارزیابی تعطیلات و نیمه‌روزها را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task DateExceptions_Should_Persist_And_Evaluate()
    {
        await _fixture.ResetTradingCalendarsAsync();
        string marketId =
            await _fixture.CreateTradingCalendarMarketAsync();

        string id =
            await CreateCalendarAsync(
                marketId,
                2026,
                32,
                DefaultSessions(),
                DefaultExceptions());

        using HttpResponseMessage holidayResponse =
            await _client.GetAsync(
                $"/api/v1/trading-calendars/{id}/days/2026-09-07");

        using JsonDocument holidayDocument =
            await ReadJsonAsync(holidayResponse);

        Assert.False(
            holidayDocument.RootElement
                .GetProperty("isTradingDay")
                .GetBoolean());

        using HttpResponseMessage halfDayResponse =
            await _client.GetAsync(
                $"/api/v1/trading-calendars/{id}/days/2026-09-08");

        using JsonDocument halfDayDocument =
            await ReadJsonAsync(halfDayResponse);

        Assert.True(
            halfDayDocument.RootElement
                .GetProperty("isTradingDay")
                .GetBoolean());

        Assert.True(
            halfDayDocument.RootElement
                .GetProperty("isHalfDay")
                .GetBoolean());
    }

    /// <summary>
    /// EN: Verifies one calendar per market/year.
    /// FA: قاعده یک تقویم برای هر Market/Year را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Duplicate_Market_Year_Should_Return_Conflict()
    {
        await _fixture.ResetTradingCalendarsAsync();
        string marketId =
            await _fixture.CreateTradingCalendarMarketAsync();

        await CreateCalendarAsync(
            marketId,
            2026,
            32,
            DefaultSessions(),
            []);

        using HttpResponseMessage response =
            await PostCalendarAsync(
                marketId,
                2026,
                32,
                DefaultSessions(),
                []);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(
            response,
            "TradingCalendar.DuplicateMarketYear");
    }

    /// <summary>
    /// EN: Verifies replacing weekend days and weekly sessions.
    /// FA: جایگزینی روزهای آخرهفته و Sessionهای هفتگی را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Update_Should_Replace_Weekend_And_Sessions()
    {
        await _fixture.ResetTradingCalendarsAsync();
        string marketId =
            await _fixture.CreateTradingCalendarMarketAsync();

        string id =
            await CreateCalendarAsync(
                marketId,
                2026,
                32,
                DefaultSessions(),
                []);

        UpdateCalendarRequest request = new(
            96,
            [
                new SessionRequest(0, new TimeOnly(10, 0), new TimeOnly(14, 0)),
                new SessionRequest(1, new TimeOnly(10, 0), new TimeOnly(14, 0))
            ],
            []);

        using HttpResponseMessage updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/v1/trading-calendars/{id}",
                request);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        using HttpResponseMessage getResponse =
            await _client.GetAsync(
                $"/api/v1/trading-calendars/{id}");

        using JsonDocument document =
            await ReadJsonAsync(getResponse);

        Assert.Equal(
            96,
            document.RootElement
                .GetProperty("weekendDays")
                .GetInt32());

        Assert.Equal(
            2,
            document.RootElement
                .GetProperty("sessions")
                .GetArrayLength());
    }

    /// <summary>
    /// EN: Verifies lifecycle idempotency and pagination normalization.
    /// FA: idempotent بودن چرخه فعال/غیرفعال و نرمال‌سازی Pagination را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Lifecycle_And_Pagination_Should_Work()
    {
        await _fixture.ResetTradingCalendarsAsync();
        string marketId =
            await _fixture.CreateTradingCalendarMarketAsync();

        string id =
            await CreateCalendarAsync(
                marketId,
                2026,
                32,
                DefaultSessions(),
                []);

        using HttpResponseMessage deactivate =
            await _client.PatchAsync(
                $"/api/v1/trading-calendars/{id}/deactivate",
                null);

        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);

        using HttpResponseMessage deactivateAgain =
            await _client.PatchAsync(
                $"/api/v1/trading-calendars/{id}/deactivate",
                null);

        Assert.Equal(HttpStatusCode.OK, deactivateAgain.StatusCode);

        using HttpResponseMessage activate =
            await _client.PatchAsync(
                $"/api/v1/trading-calendars/{id}/activate",
                null);

        Assert.Equal(HttpStatusCode.OK, activate.StatusCode);

        using HttpResponseMessage pageResponse =
            await _client.GetAsync(
                "/api/v1/trading-calendars?page=-1&pageSize=-1");

        using JsonDocument pageDocument =
            await ReadJsonAsync(pageResponse);

        Assert.Equal(
            1,
            pageDocument.RootElement.GetProperty("page").GetInt32());

        Assert.Equal(
            20,
            pageDocument.RootElement.GetProperty("pageSize").GetInt32());
    }

    /// <summary>
    /// EN: Verifies invalid identifiers and invalid session configuration.
    /// FA: شناسه نامعتبر و تنظیم نامعتبر Session را بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task Invalid_Requests_Should_Return_BadRequest()
    {
        await _fixture.ResetTradingCalendarsAsync();

        using HttpResponseMessage invalidIdResponse =
            await _client.GetAsync(
                "/api/v1/trading-calendars/abc");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            invalidIdResponse.StatusCode);

        string marketId =
            await _fixture.CreateTradingCalendarMarketAsync();

        SessionRequest[] sessions =
        [
            new SessionRequest(
                5,
                new TimeOnly(9, 0),
                new TimeOnly(12, 0))
        ];

        using HttpResponseMessage invalidSessionResponse =
            await PostCalendarAsync(
                marketId,
                2026,
                32,
                sessions,
                []);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            invalidSessionResponse.StatusCode);

        await AssertProblemCodeAsync(
            invalidSessionResponse,
            "TradingCalendar.InvalidConfiguration");
    }

    private async Task<string> CreateCalendarAsync(
        string marketId,
        int year,
        int weekendDays,
        IReadOnlyCollection<SessionRequest> sessions,
        IReadOnlyCollection<DateExceptionRequest> exceptions)
    {
        using HttpResponseMessage response =
            await PostCalendarAsync(
                marketId,
                year,
                weekendDays,
                sessions,
                exceptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        string? id =
            document.RootElement
                .GetProperty("id")
                .GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private Task<HttpResponseMessage> PostCalendarAsync(
        string marketId,
        int year,
        int weekendDays,
        IReadOnlyCollection<SessionRequest> sessions,
        IReadOnlyCollection<DateExceptionRequest> exceptions)
    {
        CreateCalendarRequest request = new(
            marketId,
            year,
            weekendDays,
            sessions,
            exceptions);

        return _client.PostAsJsonAsync(
            "/api/v1/trading-calendars",
            request);
    }

    private static SessionRequest[] DefaultSessions()
        =>
        [
            new SessionRequest(0, new TimeOnly(9, 0), new TimeOnly(15, 0)),
            new SessionRequest(1, new TimeOnly(9, 0), new TimeOnly(15, 0)),
            new SessionRequest(2, new TimeOnly(9, 0), new TimeOnly(15, 0)),
            new SessionRequest(3, new TimeOnly(9, 0), new TimeOnly(15, 0)),
            new SessionRequest(4, new TimeOnly(9, 0), new TimeOnly(15, 0)),
            new SessionRequest(6, new TimeOnly(9, 0), new TimeOnly(15, 0))
        ];

    private static DateExceptionRequest[] DefaultExceptions()
        =>
        [
            new DateExceptionRequest(
                new DateOnly(2026, 9, 7),
                true,
                false,
                "Test holiday"),
            new DateExceptionRequest(
                new DateOnly(2026, 9, 8),
                false,
                true,
                "Test half-day")
        ];

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

    private sealed record SessionRequest(
        int DayOfWeek,
        TimeOnly OpensAt,
        TimeOnly ClosesAt);

    private sealed record DateExceptionRequest(
        DateOnly Date,
        bool IsHoliday,
        bool IsHalfDay,
        string? Description);

    private sealed record CreateCalendarRequest(
        string MarketId,
        int Year,
        int WeekendDays,
        IReadOnlyCollection<SessionRequest> Sessions,
        IReadOnlyCollection<DateExceptionRequest> DateExceptions);

    private sealed record UpdateCalendarRequest(
        int WeekendDays,
        IReadOnlyCollection<SessionRequest> Sessions,
        IReadOnlyCollection<DateExceptionRequest> DateExceptions);
}
