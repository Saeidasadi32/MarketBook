// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Financial.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Globalization;

namespace MarketBook.Domain.Financial.ValueObjects;

/// <summary>
/// EN: Represents a mathematical ratio with comparison and arithmetic operations.
/// FA: یک نسبت ریاضی با عملیات مقایسه و حسابی را نمایش می‌دهد.
/// </summary>
public readonly record struct Ratio :
    IComparable<Ratio>,
    IComparable
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Ratio"/> struct.
    /// FA: یک نمونه جدید از <see cref="Ratio"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Ratio value (must be non-negative).
    /// FA: مقدار نسبت (باید غیرمنفی باشد).
    /// </param>
    public Ratio(decimal value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Ratio cannot be negative.");

        Value = decimal.Round(value, 6, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// EN: Gets the ratio value.
    /// FA: مقدار نسبت را دریافت می‌کند.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// EN: Represents a zero ratio.
    /// FA: نسبت صفر.
    /// </summary>
    public static Ratio Zero => new(0m);

    /// <summary>
    /// EN: Represents a ratio of 1.0.
    /// FA: نسبت ۱.۰.
    /// </summary>
    public static Ratio One => new(1m);

    /// <summary>
    /// EN: Gets a value indicating whether the ratio is zero.
    /// FA: مشخص می‌کند نسبت صفر است یا خیر.
    /// </summary>
    public bool IsZero => Value == 0m;

    /// <summary>
    /// EN: Gets a value indicating whether the ratio is positive.
    /// FA: مشخص می‌کند نسبت مثبت است یا خیر.
    /// </summary>
    public bool IsPositive => Value > 0m;

    /// <summary>
    /// EN: Compares the current ratio with another ratio.
    /// FA: نسبت جاری را با نسبت دیگر مقایسه می‌کند.
    /// </summary>
    public int CompareTo(Ratio other)
        => Value.CompareTo(other.Value);

    /// <summary>
    /// EN: Compares the current ratio with another object.
    /// FA: نسبت جاری را با شیء دیگر مقایسه می‌کند.
    /// </summary>
    public int CompareTo(object? obj)
    {
        if (obj is Ratio other)
            return CompareTo(other);

        throw new ArgumentException($"Object must be of type {nameof(Ratio)}.");
    }

    /// <summary>
    /// EN: Returns the string representation of the ratio.
    /// FA: نمایش رشته‌ای نسبت را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => Value.ToString("0.######", CultureInfo.InvariantCulture);

    /// <summary>
    /// EN: Parses a string to a ratio.
    /// FA: یک رشته را به نسبت تبدیل می‌کند.
    /// </summary>
    public static Ratio Parse(string value)
        => new(decimal.Parse(value, CultureInfo.InvariantCulture));

    /// <summary>
    /// EN: Tries to parse a string to a ratio.
    /// FA: سعی می‌کند یک رشته را به نسبت تبدیل کند.
    /// </summary>
    public static bool TryParse(string value, out Ratio ratio)
    {
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result) && result >= 0)
        {
            ratio = new(result);
            return true;
        }

        ratio = Zero;
        return false;
    }

    /// <summary>
    /// EN: Converts the ratio to a percentage.
    /// FA: نسبت را به درصد تبدیل می‌کند.
    /// </summary>
    public Percentage ToPercentage()
        => new(Value * 100m);

    // Operators
    public static Ratio operator +(Ratio left, Ratio right)
        => new(left.Value + right.Value);

    public static Ratio operator -(Ratio left, Ratio right)
    {
        var result = left.Value - right.Value;
        if (result < 0)
            throw new InvalidOperationException("Ratio cannot be negative.");
        return new(result);
    }

    public static Ratio operator *(Ratio left, decimal factor)
        => new(left.Value * factor);

    public static Ratio operator /(Ratio left, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException();
        return new(left.Value / divisor);
    }

    public static bool operator >(Ratio left, Ratio right)
        => left.Value > right.Value;

    public static bool operator <(Ratio left, Ratio right)
        => left.Value < right.Value;

    public static bool operator >=(Ratio left, Ratio right)
        => left.Value >= right.Value;

    public static bool operator <=(Ratio left, Ratio right)
        => left.Value <= right.Value;

    public static implicit operator decimal(Ratio ratio)
        => ratio.Value;

    public static explicit operator Ratio(decimal value)
        => new(value);
}