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
/// EN: Represents a percentage value with validation and operations.
/// FA: یک مقدار درصد با اعتبارسنجی و عملیات را نمایش می‌دهد.
/// </summary>
public readonly record struct Percentage :
    IComparable<Percentage>,
    IComparable
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Percentage"/> struct.
    /// FA: یک نمونه جدید از <see cref="Percentage"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Percentage value (e.g., 12.5 for 12.5%).
    /// FA: مقدار درصد (مثلاً 12.5 برای ۱۲.۵٪).
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// EN: Thrown when the value is outside the valid range [-100, 100].
    /// FA: زمانی که مقدار خارج از محدوده معتبر [-۱۰۰, ۱۰۰] باشد پرتاب می‌شود.
    /// </exception>
    public Percentage(decimal value)
    {
        if (value < -100m || value > 100m)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Percentage must be between -100 and 100.");

        Value = decimal.Round(value, 4, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// EN: Gets the percentage value.
    /// FA: مقدار درصد را دریافت می‌کند.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// EN: Represents zero percent.
    /// FA: درصد صفر.
    /// </summary>
    public static Percentage Zero => new(0m);

    /// <summary>
    /// EN: Represents 100 percent.
    /// FA: ۱۰۰ درصد.
    /// </summary>
    public static Percentage OneHundred => new(100m);

    /// <summary>
    /// EN: Gets a value indicating whether the percentage is zero.
    /// FA: مشخص می‌کند که درصد صفر است یا خیر.
    /// </summary>
    public bool IsZero => Value == 0m;

    /// <summary>
    /// EN: Gets a value indicating whether the percentage is positive.
    /// FA: مشخص می‌کند که درصد مثبت است یا خیر.
    /// </summary>
    public bool IsPositive => Value > 0m;

    /// <summary>
    /// EN: Gets a value indicating whether the percentage is negative.
    /// FA: مشخص می‌کند که درصد منفی است یا خیر.
    /// </summary>
    public bool IsNegative => Value < 0m;

    /// <summary>
    /// EN: Converts the percentage to a factor (e.g., 12.5% → 0.125).
    /// FA: درصد را به ضریب تبدیل می‌کند (مثلاً ۱۲.۵٪ → ۰.۱۲۵).
    /// </summary>
    public decimal AsFactor() => Value / 100m;

    /// <summary>
    /// EN: Converts a factor to a percentage (e.g., 0.125 → 12.5%).
    /// FA: ضریب را به درصد تبدیل می‌کند (مثلاً ۰.۱۲۵ → ۱۲.۵٪).
    /// </summary>
    public static Percentage FromFactor(decimal factor)
        => new(factor * 100m);

    /// <summary>
    /// EN: Compares the current percentage with another percentage.
    /// FA: درصد جاری را با درصد دیگر مقایسه می‌کند.
    /// </summary>
    /// <param name="other">The other percentage.</param>
    /// <returns>A value indicating the relative order.</returns>
    public int CompareTo(Percentage other)
        => Value.CompareTo(other.Value);

    /// <summary>
    /// EN: Compares the current percentage with another object.
    /// FA: درصد جاری را با شیء دیگر مقایسه می‌کند.
    /// </summary>
    public int CompareTo(object? obj)
    {
        if (obj is Percentage other)
            return CompareTo(other);

        throw new ArgumentException($"Object must be of type {nameof(Percentage)}.");
    }

    /// <summary>
    /// EN: Returns the string representation of the percentage.
    /// FA: نمایش رشته‌ای درصد را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => $"{Value:0.####}%";

    /// <summary>
    /// EN: Parses a string to a percentage.
    /// FA: یک رشته را به درصد تبدیل می‌کند.
    /// </summary>
    public static Percentage Parse(string value)
    {
        var clean = value.TrimEnd('%', ' ');
        return new Percentage(decimal.Parse(clean, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// EN: Tries to parse a string to a percentage.
    /// FA: سعی می‌کند یک رشته را به درصد تبدیل کند.
    /// </summary>
    public static bool TryParse(string value, out Percentage percentage)
    {
        try
        {
            percentage = Parse(value);
            return true;
        }
        catch
        {
            percentage = Zero;
            return false;
        }
    }

    // Operators
    public static Percentage operator +(Percentage left, Percentage right)
        => new(left.Value + right.Value);

    public static Percentage operator -(Percentage left, Percentage right)
        => new(left.Value - right.Value);

    public static Percentage operator *(Percentage left, decimal factor)
        => new(left.Value * factor);

    public static Percentage operator /(Percentage left, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException();
        return new Percentage(left.Value / divisor);
    }

    public static bool operator >(Percentage left, Percentage right)
        => left.Value > right.Value;

    public static bool operator <(Percentage left, Percentage right)
        => left.Value < right.Value;

    public static bool operator >=(Percentage left, Percentage right)
        => left.Value >= right.Value;

    public static bool operator <=(Percentage left, Percentage right)
        => left.Value <= right.Value;

    public static implicit operator decimal(Percentage percentage)
        => percentage.Value;

    public static explicit operator Percentage(decimal value)
        => new(value);

    public Percentage Abs() => new(decimal.Abs(Value));
}