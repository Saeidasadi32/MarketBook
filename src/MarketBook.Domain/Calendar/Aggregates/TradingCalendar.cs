// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Calendar.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Calendar.ValueObjects;
using MarketBook.Domain.Common;
using MarketBook.Domain.Market.ValueObjects;

namespace MarketBook.Domain.Calendar.Aggregates;

/// <summary>
/// EN: Represents a trading calendar for a market.
/// FA: تقویم معاملاتی یک بازار را نمایش می‌دهد.
/// </summary>
public sealed class TradingCalendar : AggregateRoot<TradingCalendarId>
{
    private readonly HashSet<DateOnly> _holidays = [];
    private readonly HashSet<DateOnly> _halfDays = [];

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="TradingCalendar"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="TradingCalendar"/> را ایجاد می‌کند.
    /// </summary>
    public TradingCalendar(
        TradingCalendarId id,
        MarketId marketId,
        int year)
        : base(id)
    {
        MarketId = marketId;
        Year = year;
        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private TradingCalendar()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets the market identifier.
    /// FA: شناسه بازار را دریافت می‌کند.
    /// </summary>
    public MarketId MarketId { get; }

    /// <summary>
    /// EN: Gets the calendar year.
    /// FA: سال تقویم را دریافت می‌کند.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// EN: Gets the creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets a value indicating whether the calendar is active.
    /// FA: مشخص می‌کند تقویم فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Gets all holidays.
    /// FA: همه تعطیلات را دریافت می‌کند.
    /// </summary>
    public IReadOnlySet<DateOnly> Holidays => _holidays;

    /// <summary>
    /// EN: Gets all half-day trading days.
    /// FA: همه روزهای معاملاتی نیمه‌روز را دریافت می‌کند.
    /// </summary>
    public IReadOnlySet<DateOnly> HalfDays => _halfDays;

    /// <summary>
    /// EN: Adds a holiday.
    /// FA: یک تعطیلی اضافه می‌کند.
    /// </summary>
    public void AddHoliday(DateOnly date, string? description = null)
    {
        if (_holidays.Add(date))
        {
            AddDomainEvent(new HolidayAddedEvent(Id, date, description));
        }
    }

    /// <summary>
    /// EN: Removes a holiday.
    /// FA: یک تعطیلی را حذف می‌کند.
    /// </summary>
    public void RemoveHoliday(DateOnly date)
    {
        if (_holidays.Remove(date))
        {
            AddDomainEvent(new HolidayRemovedEvent(Id, date));
        }
    }

    /// <summary>
    /// EN: Adds a half-day trading day.
    /// FA: یک روز معاملاتی نیمه‌روز اضافه می‌کند.
    /// </summary>
    public void AddHalfDay(DateOnly date, string? description = null)
    {
        if (_halfDays.Add(date))
        {
            AddDomainEvent(new HalfDayAddedEvent(Id, date, description));
        }
    }

    /// <summary>
    /// EN: Removes a half-day trading day.
    /// FA: یک روز معاملاتی نیمه‌روز را حذف می‌کند.
    /// </summary>
    public void RemoveHalfDay(DateOnly date)
    {
        if (_halfDays.Remove(date))
        {
            AddDomainEvent(new HalfDayRemovedEvent(Id, date));
        }
    }

    /// <summary>
    /// EN: Determines whether a date is a trading day.
    /// FA: مشخص می‌کند که یک تاریخ روز معاملاتی است یا خیر.
    /// </summary>
    public bool IsTradingDay(DateOnly date)
    {
        if (date.Year != Year)
            return false;

        if (_holidays.Contains(date))
            return false;

        // Weekends (Saturday and Sunday)
        if (date.DayOfWeek == DayOfWeek.Saturday ||
            date.DayOfWeek == DayOfWeek.Sunday)
            return false;

        return true;
    }

    /// <summary>
    /// EN: Determines whether a date is a half-day.
    /// FA: مشخص می‌کند که یک تاریخ روز نیمه‌روز است یا خیر.
    /// </summary>
    public bool IsHalfDay(DateOnly date)
        => _halfDays.Contains(date);

    /// <summary>
    /// EN: Gets the next trading day.
    /// FA: روز معاملاتی بعدی را دریافت می‌کند.
    /// </summary>
    public DateOnly? GetNextTradingDay(DateOnly fromDate)
    {
        var date = fromDate.AddDays(1);
        var maxAttempts = 365;

        while (maxAttempts-- > 0)
        {
            if (IsTradingDay(date))
                return date;

            date = date.AddDays(1);
        }

        return null;
    }

    /// <summary>
    /// EN: Gets all trading days in the year.
    /// FA: همه روزهای معاملاتی سال را دریافت می‌کند.
    /// </summary>
    public IEnumerable<DateOnly> GetTradingDays()
    {
        for (var month = 1; month <= 12; month++)
        {
            var daysInMonth = DateTime.DaysInMonth(Year, month);
            for (var day = 1; day <= daysInMonth; day++)
            {
                var date = new DateOnly(Year, month, day);
                if (IsTradingDay(date))
                {
                    yield return date;
                }
            }
        }
    }

    /// <summary>
    /// EN: Activates the calendar.
    /// FA: تقویم را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        AddDomainEvent(new CalendarActivatedEvent(Id));
    }

    /// <summary>
    /// EN: Deactivates the calendar.
    /// FA: تقویم را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        AddDomainEvent(new CalendarDeactivatedEvent(Id));
    }
}

// Domain Events
public sealed record HolidayAddedEvent(
    TradingCalendarId CalendarId,
    DateOnly Date,
    string? Description) : DomainEvent;

public sealed record HolidayRemovedEvent(
    TradingCalendarId CalendarId,
    DateOnly Date) : DomainEvent;

public sealed record HalfDayAddedEvent(
    TradingCalendarId CalendarId,
    DateOnly Date,
    string? Description) : DomainEvent;

public sealed record HalfDayRemovedEvent(
    TradingCalendarId CalendarId,
    DateOnly Date) : DomainEvent;

public sealed record CalendarActivatedEvent(
    TradingCalendarId CalendarId) : DomainEvent;

public sealed record CalendarDeactivatedEvent(
    TradingCalendarId CalendarId) : DomainEvent;