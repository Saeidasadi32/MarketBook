// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Financial.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common.ValueObjects;
using System.Globalization;

namespace MarketBook.Domain.Financial.ValueObjects;

/// <summary>
/// EN: Represents a percentage change with comparison operations.
/// FA: درصد تغییر با عملیات مقایسه را نمایش می‌دهد.
/// </summary>
public readonly record struct PercentageChange :
    IComparable<PercentageChange>,
    IComparable
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="PercentageChange"/> struct.
    /// FA: یک نمونه جدید از <see cref="PercentageChange"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Percentage change value (e.g., 12.5 for +12.5%).
    /// FA: مقدار درصد تغییر (مثلاً ۱۲.۵ برای +۱۲.۵٪).
    /// </param>
    public PercentageChange(decimal value)
    {
        Value = decimal.Round(value, 4, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// EN: Initializes a new instance from a Percentage.
    /// FA: یک نمونه جدید از یک Percentage ایجاد می‌کند.
    /// </summary>
    public PercentageChange(Percentage percentage)
    {
        Value = percentage.Value;
    }

    /// <summary>
    /// EN: Gets the percentage change value.
    /// FA: مقدار درصد تغییر را دریافت می‌کند.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// EN: Represents zero percent change.
    /// FA: تغییر صفر درصد.
    /// </summary>
    public static PercentageChange Zero => new(0m);

    /// <summary>
    /// EN: Gets a value indicating whether the change is positive.
    /// FA: مشخص می‌کند تغییر مثبت است یا خیر.
    /// </summary>
    public bool IsPositive => Value > 0m;

    /// <summary>
    /// EN: Gets a value indicating whether the change is negative.
    /// FA: مشخص می‌کند تغییر منفی است یا خیر.
    /// </summary>
    public bool IsNegative => Value < 0m;

    /// <summary>
    /// EN: Gets a value indicating whether the change is zero.
    /// FA: مشخص می‌کند تغییر صفر است یا خیر.
    /// </summary>
    public bool IsZero => Value == 0m;

    /// <summary>
    /// EN: Converts to a Percentage.
    /// FA: به Percentage تبدیل می‌کند.
    /// </summary>
    public Percentage ToPercentage() => new(Value);

    /// <summary>
    /// EN: Compares the current change with another change.
    /// FA: تغییر جاری را با تغییر دیگر مقایسه می‌کند.
    /// </summary>
    public int CompareTo(PercentageChange other)
        => Value.CompareTo(other.Value);

    /// <summary>
    /// EN: Compares the current change with another object.
    /// FA: تغییر جاری را با شیء دیگر مقایسه می‌کند.
    /// </summary>
    public int CompareTo(object? obj)
    {
        if (obj is PercentageChange other)
            return CompareTo(other);

        throw new ArgumentException($"Object must be of type {nameof(PercentageChange)}.");
    }

    /// <summary>
    /// EN: Returns the string representation.
    /// FA: نمایش رشته‌ای را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => $"{Value:0.####}%";

    /// <summary>
    /// EN: Calculates the percentage change between two values.
    /// FA: درصد تغییر بین دو مقدار را محاسبه می‌کند.
    /// </summary>
    public static PercentageChange Calculate(decimal oldValue, decimal newValue)
    {
        if (oldValue == 0)
            return Zero;

        var change = (newValue - oldValue) / oldValue * 100m;
        return new PercentageChange(change);
    }

    /// <summary>
    /// EN: Calculates the percentage change between two prices.
    /// FA: درصد تغییر بین دو قیمت را محاسبه می‌کند.
    /// </summary>
    public static PercentageChange Calculate(Price oldPrice, Price newPrice)
        => Calculate(oldPrice.Value, newPrice.Value);

    // Operators
    public static bool operator >(PercentageChange left, PercentageChange right)
        => left.Value > right.Value;

    public static bool operator <(PercentageChange left, PercentageChange right)
        => left.Value < right.Value;

    public static bool operator >=(PercentageChange left, PercentageChange right)
        => left.Value >= right.Value;

    public static bool operator <=(PercentageChange left, PercentageChange right)
        => left.Value <= right.Value;

    public static implicit operator decimal(PercentageChange change)
        => change.Value;

    public static explicit operator PercentageChange(decimal value)
        => new(value);

    public static implicit operator Percentage(PercentageChange change)
        => new(change.Value);

    public static explicit operator PercentageChange(Percentage percentage)
        => new(percentage);
}