// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Calendar.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Calendar.Entities;
using MarketBook.Domain.Calendar.Enums;
using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MarketBook.Domain.Market.ValueObjects;

namespace MarketBook.Domain.Calendar.Aggregates;

/// <summary>
/// EN: Represents the yearly trading calendar and regular sessions of a market.
/// FA: تقویم معاملاتی سالانه و Sessionهای عادی یک بازار را نمایش می‌دهد.
/// </summary>
public sealed class TradingCalendar : AggregateRoot<TradingCalendarId>
{
    private readonly List<CalendarDateException> _dateExceptions = [];
    private readonly List<TradingSession> _sessions = [];

    /// <summary>
    /// EN: Initializes a trading calendar with configurable weekend days.
    /// FA: تقویم معاملاتی را با روزهای آخرهفته قابل تنظیم ایجاد می‌کند.
    /// </summary>
    public TradingCalendar(
        TradingCalendarId id,
        MarketId marketId,
        int year,
        TradingWeekDays weekendDays)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(marketId);
        ValidateYear(year);
        ValidateWeekendDays(weekendDays);

        MarketId = marketId;
        Year = year;
        WeekendDays = weekendDays;
        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    private TradingCalendar()
    {
    }

    /// <summary>
    /// EN: Gets the market identifier.
    /// FA: شناسه بازار را دریافت می‌کند.
    /// </summary>
    public MarketId MarketId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the Gregorian calendar year.
    /// FA: سال میلادی تقویم را دریافت می‌کند.
    /// </summary>
    public int Year { get; private set; }

    /// <summary>
    /// EN: Gets the configured non-trading weekend days.
    /// FA: روزهای آخرهفته غیرمعاملاتی پیکربندی‌شده را دریافت می‌کند.
    /// </summary>
    public TradingWeekDays WeekendDays { get; private set; }

    /// <summary>
    /// EN: Gets the creation timestamp.
    /// FA: زمان ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// EN: Gets whether the calendar is active.
    /// FA: مشخص می‌کند تقویم فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Gets date-specific holidays and half-days.
    /// FA: تعطیلات و نیمه‌روزهای تاریخ‌محور را دریافت می‌کند.
    /// </summary>
    public IReadOnlyCollection<CalendarDateException> DateExceptions
        => _dateExceptions.AsReadOnly();

    /// <summary>
    /// EN: Gets regular weekly trading sessions.
    /// FA: Sessionهای معاملاتی عادی هفتگی را دریافت می‌کند.
    /// </summary>
    public IReadOnlyCollection<TradingSession> Sessions
        => _sessions.AsReadOnly();

    /// <summary>
    /// EN: Changes the market-specific weekend configuration.
    /// FA: تنظیم روزهای آخرهفته بازار را تغییر می‌دهد.
    /// </summary>
    public void ChangeWeekendDays(TradingWeekDays weekendDays)
    {
        ValidateWeekendDays(weekendDays);
        WeekendDays = weekendDays;
    }

    /// <summary>
    /// EN: Replaces the regular weekly trading sessions.
    /// FA: Sessionهای معاملاتی عادی هفتگی را جایگزین می‌کند.
    /// </summary>
    public void ReplaceSessions(IEnumerable<TradingSession> sessions)
    {
        ArgumentNullException.ThrowIfNull(sessions);

        List<TradingSession> normalizedSessions = sessions.ToList();

        if (normalizedSessions
            .GroupBy(session => session.DayOfWeek)
            .Any(group => group.Count() > 1))
        {
            throw new ArgumentException(
                "Only one regular trading session per week day is supported.",
                nameof(sessions));
        }

        if (normalizedSessions.Any(
            session => IsWeekend(session.DayOfWeek)))
        {
            throw new ArgumentException(
                "A regular trading session cannot be configured on a weekend day.",
                nameof(sessions));
        }

        _sessions.Clear();
        _sessions.AddRange(normalizedSessions);
    }

    /// <summary>
    /// EN: Adds or replaces a holiday.
    /// FA: یک تعطیلی را اضافه یا جایگزین می‌کند.
    /// </summary>
    public void AddHoliday(
        DateOnly date,
        string? description = null)
    {
        ValidateDate(date);
        RemoveDateException(date);

        _dateExceptions.Add(
            new CalendarDateException(
                date,
                true,
                false,
                description));

        Raise(new HolidayAddedEvent(Id, date, description));
    }

    /// <summary>
    /// EN: Adds or replaces a half-day.
    /// FA: یک نیمه‌روز را اضافه یا جایگزین می‌کند.
    /// </summary>
    public void AddHalfDay(
        DateOnly date,
        string? description = null)
    {
        ValidateDate(date);

        if (IsWeekend(date.DayOfWeek))
        {
            throw new ArgumentException(
                "A half-day cannot be configured on a weekend date.",
                nameof(date));
        }

        RemoveDateException(date);

        _dateExceptions.Add(
            new CalendarDateException(
                date,
                false,
                true,
                description));

        Raise(new HalfDayAddedEvent(Id, date, description));
    }

    /// <summary>
    /// EN: Removes any holiday or half-day exception for a date.
    /// FA: هر استثنای تعطیلی یا نیمه‌روز را برای یک تاریخ حذف می‌کند.
    /// </summary>
    public void RemoveDateException(DateOnly date)
    {
        CalendarDateException? existing =
            _dateExceptions.SingleOrDefault(item => item.Date == date);

        if (existing is not null)
        {
            _dateExceptions.Remove(existing);
        }
    }

    /// <summary>
    /// EN: Determines whether the specified date is a trading day.
    /// FA: مشخص می‌کند تاریخ موردنظر روز معاملاتی است یا خیر.
    /// </summary>
    public bool IsTradingDay(DateOnly date)
    {
        if (date.Year != Year)
        {
            return false;
        }

        CalendarDateException? exception =
            _dateExceptions.SingleOrDefault(item => item.Date == date);

        if (exception?.IsHoliday == true)
        {
            return false;
        }

        return !IsWeekend(date.DayOfWeek);
    }

    /// <summary>
    /// EN: Determines whether the specified date is a half-day.
    /// FA: مشخص می‌کند تاریخ موردنظر نیمه‌روز است یا خیر.
    /// </summary>
    public bool IsHalfDay(DateOnly date)
        => _dateExceptions.Any(
            item => item.Date == date && item.IsHalfDay);

    /// <summary>
    /// EN: Gets the regular session for a date when it is a trading day.
    /// FA: در صورت معاملاتی بودن تاریخ، Session عادی آن روز را دریافت می‌کند.
    /// </summary>
    public TradingSession? GetSession(DateOnly date)
    {
        if (!IsTradingDay(date))
        {
            return null;
        }

        return _sessions.SingleOrDefault(
            session => session.DayOfWeek == date.DayOfWeek);
    }

    /// <summary>
    /// EN: Gets the next trading day inside this yearly calendar.
    /// FA: روز معاملاتی بعدی را در محدوده همین تقویم سالانه دریافت می‌کند.
    /// </summary>
    public DateOnly? GetNextTradingDay(DateOnly fromDate)
    {
        DateOnly date = fromDate.AddDays(1);

        while (date.Year == Year)
        {
            if (IsTradingDay(date))
            {
                return date;
            }

            date = date.AddDays(1);
        }

        return null;
    }

    /// <summary>
    /// EN: Activates the calendar.
    /// FA: تقویم را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        Raise(new CalendarActivatedEvent(Id));
    }

    /// <summary>
    /// EN: Deactivates the calendar.
    /// FA: تقویم را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        Raise(new CalendarDeactivatedEvent(Id));
    }

    /// <summary>
    /// EN: Creates a new yearly trading calendar.
    /// FA: یک تقویم معاملاتی سالانه جدید ایجاد می‌کند.
    /// </summary>
    public static TradingCalendar Create(
        MarketId marketId,
        int year,
        TradingWeekDays weekendDays)
        => new(
            TradingCalendarId.New(),
            marketId,
            year,
            weekendDays);

    private bool IsWeekend(DayOfWeek dayOfWeek)
    {
        TradingWeekDays flag = dayOfWeek switch
        {
            DayOfWeek.Sunday => TradingWeekDays.Sunday,
            DayOfWeek.Monday => TradingWeekDays.Monday,
            DayOfWeek.Tuesday => TradingWeekDays.Tuesday,
            DayOfWeek.Wednesday => TradingWeekDays.Wednesday,
            DayOfWeek.Thursday => TradingWeekDays.Thursday,
            DayOfWeek.Friday => TradingWeekDays.Friday,
            DayOfWeek.Saturday => TradingWeekDays.Saturday,
            _ => TradingWeekDays.None
        };

        return WeekendDays.HasFlag(flag);
    }

    private void ValidateDate(DateOnly date)
    {
        if (date.Year != Year)
        {
            throw new ArgumentOutOfRangeException(
                nameof(date),
                "The date must belong to the calendar year.");
        }
    }

    private static void ValidateYear(int year)
    {
        if (year < 1900 || year > 2200)
        {
            throw new ArgumentOutOfRangeException(
                nameof(year),
                "Calendar year must be between 1900 and 2200.");
        }
    }

    private static void ValidateWeekendDays(TradingWeekDays weekendDays)
    {
        TradingWeekDays allDays =
            TradingWeekDays.Sunday |
            TradingWeekDays.Monday |
            TradingWeekDays.Tuesday |
            TradingWeekDays.Wednesday |
            TradingWeekDays.Thursday |
            TradingWeekDays.Friday |
            TradingWeekDays.Saturday;

        if ((weekendDays & ~allDays) != TradingWeekDays.None)
        {
            throw new ArgumentOutOfRangeException(
                nameof(weekendDays),
                "Weekend days contain unsupported flags.");
        }

        if (weekendDays == allDays)
        {
            throw new ArgumentException(
                "At least one week day must remain available for trading.",
                nameof(weekendDays));
        }
    }
}

/// <summary>
/// EN: Raised when a holiday is configured.
/// FA: هنگام ثبت تعطیلی ایجاد می‌شود.
/// </summary>
public sealed record HolidayAddedEvent(
    TradingCalendarId CalendarId,
    DateOnly Date,
    string? Description) : DomainEvent;

/// <summary>
/// EN: Raised when a half-day is configured.
/// FA: هنگام ثبت نیمه‌روز ایجاد می‌شود.
/// </summary>
public sealed record HalfDayAddedEvent(
    TradingCalendarId CalendarId,
    DateOnly Date,
    string? Description) : DomainEvent;

/// <summary>
/// EN: Raised when a calendar is activated.
/// FA: هنگام فعال شدن تقویم ایجاد می‌شود.
/// </summary>
public sealed record CalendarActivatedEvent(
    TradingCalendarId CalendarId) : DomainEvent;

/// <summary>
/// EN: Raised when a calendar is deactivated.
/// FA: هنگام غیرفعال شدن تقویم ایجاد می‌شود.
/// </summary>
public sealed record CalendarDeactivatedEvent(
    TradingCalendarId CalendarId) : DomainEvent;
