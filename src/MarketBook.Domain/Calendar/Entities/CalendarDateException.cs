// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Calendar.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Calendar.Entities;

/// <summary>
/// EN: Represents a holiday or half-day exception for one calendar date.
/// FA: استثنای تعطیلی یا نیمه‌روز را برای یک تاریخ تقویم نمایش می‌دهد.
/// </summary>
public sealed class CalendarDateException
{
    /// <summary>
    /// EN: Initializes a date exception.
    /// FA: استثنای یک تاریخ را ایجاد می‌کند.
    /// </summary>
    public CalendarDateException(
        DateOnly date,
        bool isHoliday,
        bool isHalfDay,
        string? description = null)
    {
        if (isHoliday && isHalfDay)
        {
            throw new ArgumentException(
                "A calendar date cannot be both a holiday and a half-day.");
        }

        if (!isHoliday && !isHalfDay)
        {
            throw new ArgumentException(
                "A calendar date exception must be a holiday or a half-day.");
        }

        Date = date;
        IsHoliday = isHoliday;
        IsHalfDay = isHalfDay;
        Description = NormalizeDescription(description);
    }

    private CalendarDateException()
    {
    }

    /// <summary>
    /// EN: Gets the exceptional date.
    /// FA: تاریخ استثنا را دریافت می‌کند.
    /// </summary>
    public DateOnly Date { get; private set; }

    /// <summary>
    /// EN: Gets whether the date is a full holiday.
    /// FA: مشخص می‌کند تاریخ تعطیل کامل است یا خیر.
    /// </summary>
    public bool IsHoliday { get; private set; }

    /// <summary>
    /// EN: Gets whether the date is a half-day.
    /// FA: مشخص می‌کند تاریخ نیمه‌روز است یا خیر.
    /// </summary>
    public bool IsHalfDay { get; private set; }

    /// <summary>
    /// EN: Gets the optional description.
    /// FA: توضیح اختیاری را دریافت می‌کند.
    /// </summary>
    public string? Description { get; private set; }

    private static string? NormalizeDescription(string? description)
        => string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
}
