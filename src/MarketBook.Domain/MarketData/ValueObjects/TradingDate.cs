// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Globalization;

namespace MarketBook.Domain.MarketData.ValueObjects;

/// <summary>
/// EN: Represents a trading date.
/// FA: تاریخ معاملاتی را نمایش می‌دهد.
/// </summary>
public readonly record struct TradingDate : IComparable<TradingDate>
{
    /// <summary>
    /// EN: Initializes a new trading date.
    /// FA: یک تاریخ معاملاتی جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Trading date.
    /// FA: تاریخ معامله.
    /// </param>
    public TradingDate(DateOnly value)
    {
        Value = value;
    }

    /// <summary>
    /// EN: Initializes a new trading date from year, month, day.
    /// FA: یک تاریخ معاملاتی جدید از سال، ماه، روز ایجاد می‌کند.
    /// </summary>
    public TradingDate(int year, int month, int day)
        : this(new DateOnly(year, month, day))
    {
    }

    /// <summary>
    /// EN: Gets trading date.
    /// FA: تاریخ معاملاتی را دریافت می‌کند.
    /// </summary>
    public DateOnly Value { get; }

    /// <summary>
    /// EN: Gets the current trading date (UTC).
    /// FA: تاریخ معاملاتی جاری (UTC) را دریافت می‌کند.
    /// </summary>
    public static TradingDate Today => new(DateOnly.FromDateTime(DateTime.UtcNow));

    /// <summary>
    /// EN: Gets the year component.
    /// FA: جزء سال را دریافت می‌کند.
    /// </summary>
    public int Year => Value.Year;

    /// <summary>
    /// EN: Gets the month component.
    /// FA: جزء ماه را دریافت می‌کند.
    /// </summary>
    public int Month => Value.Month;

    /// <summary>
    /// EN: Gets the day component.
    /// FA: جزء روز را دریافت می‌کند.
    /// </summary>
    public int Day => Value.Day;

    /// <summary>
    /// EN: Gets the day of week.
    /// FA: روز هفته را دریافت می‌کند.
    /// </summary>
    public DayOfWeek DayOfWeek => Value.DayOfWeek;

    /// <summary>
    /// EN: Determines whether the date is a weekday (Monday-Friday).
    /// FA: مشخص می‌کند تاریخ یک روز کاری است یا خیر.
    /// </summary>
    public bool IsWeekday => DayOfWeek >= DayOfWeek.Monday && DayOfWeek <= DayOfWeek.Friday;

    /// <summary>
    /// EN: Determines whether the date is a weekend (Saturday-Sunday).
    /// FA: مشخص می‌کند تاریخ یک روز تعطیل است یا خیر.
    /// </summary>
    public bool IsWeekend => !IsWeekday;

    /// <summary>
    /// EN: Compares the current trading date with another.
    /// FA: تاریخ معاملاتی جاری را با دیگری مقایسه می‌کند.
    /// </summary>
    public int CompareTo(TradingDate other)
        => Value.CompareTo(other.Value);

    /// <summary>
    /// EN: Returns a new trading date with the specified number of days added.
    /// FA: یک تاریخ معاملاتی جدید با تعداد روزهای اضافه شده برمی‌گرداند.
    /// </summary>
    public TradingDate AddDays(int days)
        => new(Value.AddDays(days));

    /// <summary>
    /// EN: Returns a new trading date with the specified number of months added.
    /// FA: یک تاریخ معاملاتی جدید با تعداد ماه‌های اضافه شده برمی‌گرداند.
    /// </summary>
    public TradingDate AddMonths(int months)
        => new(Value.AddMonths(months));

    /// <summary>
    /// EN: Returns a new trading date with the specified number of years added.
    /// FA: یک تاریخ معاملاتی جدید با تعداد سال‌های اضافه شده برمی‌گرداند.
    /// </summary>
    public TradingDate AddYears(int years)
        => new(Value.AddYears(years));

    /// <summary>
    /// EN: Returns the difference in days between two trading dates.
    /// FA: اختلاف روز بین دو تاریخ معاملاتی را برمی‌گرداند.
    /// </summary>
    public int DaysSince(TradingDate other)
        => Value.DayNumber - other.Value.DayNumber;

    /// <inheritdoc/>
    public override string ToString()
        => Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    /// <summary>
    /// EN: Parses a string to a trading date.
    /// FA: یک رشته را به تاریخ معاملاتی تبدیل می‌کند.
    /// </summary>
    public static TradingDate Parse(string value)
        => new(DateOnly.Parse(value, CultureInfo.InvariantCulture));

    /// <summary>
    /// EN: Tries to parse a string to a trading date.
    /// FA: سعی می‌کند یک رشته را به تاریخ معاملاتی تبدیل کند.
    /// </summary>
    public static bool TryParse(string value, out TradingDate date)
    {
        if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, out var result))
        {
            date = new(result);
            return true;
        }

        date = default;
        return false;
    }

    /// <summary>
    /// EN: Implicit conversion to DateOnly.
    /// FA: تبدیل ضمنی به DateOnly.
    /// </summary>
    public static implicit operator DateOnly(TradingDate date)
        => date.Value;

    /// <summary>
    /// EN: Explicit conversion from DateOnly.
    /// FA: تبدیل صریح از DateOnly.
    /// </summary>
    public static explicit operator TradingDate(DateOnly value)
        => new(value);

    /// <summary>
    /// EN: Implicit conversion to DateTime.
    /// FA: تبدیل ضمنی به DateTime.
    /// </summary>
    public static implicit operator DateTime(TradingDate date)
        => date.Value.ToDateTime(TimeOnly.MinValue);

    /// <summary>
    /// EN: Explicit conversion from DateTime.
    /// FA: تبدیل صریح از DateTime.
    /// </summary>
    public static explicit operator TradingDate(DateTime value)
        => new(DateOnly.FromDateTime(value));
}