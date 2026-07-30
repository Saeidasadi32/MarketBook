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
/// EN: Represents a currency exchange rate with validation and operations.
/// FA: نرخ تبدیل ارز با اعتبارسنجی و عملیات را نمایش می‌دهد.
/// </summary>
public readonly record struct ExchangeRate :
    IComparable<ExchangeRate>,
    IComparable
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="ExchangeRate"/> struct.
    /// FA: یک نمونه جدید از <see cref="ExchangeRate"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Exchange rate value (must be greater than zero).
    /// FA: مقدار نرخ تبدیل (باید بزرگتر از صفر باشد).
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// EN: Thrown when the value is less than or equal to zero.
    /// FA: زمانی که مقدار کمتر یا مساوی صفر باشد پرتاب می‌شود.
    /// </exception>
    public ExchangeRate(decimal value)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Exchange rate must be greater than zero.");

        Value = decimal.Round(value, 6, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// EN: Gets the exchange rate value.
    /// FA: مقدار نرخ تبدیل را دریافت می‌کند.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// EN: Represents a rate of 1.0.
    /// FA: نرخ ۱.۰.
    /// </summary>
    public static ExchangeRate One => new(1m);

    /// <summary>
    /// EN: Gets a value indicating whether the rate is exactly 1.0.
    /// FA: مشخص می‌کند که نرخ دقیقاً ۱.۰ است یا خیر.
    /// </summary>
    public bool IsOne => Value == 1m;

    /// <summary>
    /// EN: Compares the current rate with another rate.
    /// FA: نرخ جاری را با نرخ دیگر مقایسه می‌کند.
    /// </summary>
    public int CompareTo(ExchangeRate other)
        => Value.CompareTo(other.Value);

    /// <summary>
    /// EN: Compares the current rate with another object.
    /// FA: نرخ جاری را با شیء دیگر مقایسه می‌کند.
    /// </summary>
    public int CompareTo(object? obj)
    {
        if (obj is ExchangeRate other)
            return CompareTo(other);

        throw new ArgumentException($"Object must be of type {nameof(ExchangeRate)}.");
    }

    /// <summary>
    /// EN: Returns the string representation of the exchange rate.
    /// FA: نمایش رشته‌ای نرخ تبدیل را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => Value.ToString("0.######", CultureInfo.InvariantCulture);

    /// <summary>
    /// EN: Parses a string to an exchange rate.
    /// FA: یک رشته را به نرخ تبدیل تبدیل می‌کند.
    /// </summary>
    public static ExchangeRate Parse(string value)
        => new(decimal.Parse(value, CultureInfo.InvariantCulture));

    /// <summary>
    /// EN: Tries to parse a string to an exchange rate.
    /// FA: سعی می‌کند یک رشته را به نرخ تبدیل تبدیل کند.
    /// </summary>
    public static bool TryParse(string value, out ExchangeRate rate)
    {
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result) && result > 0)
        {
            rate = new(result);
            return true;
        }

        rate = default;
        return false;
    }

    // Operators
    public static ExchangeRate operator +(ExchangeRate left, ExchangeRate right)
        => new(left.Value + right.Value);

    public static ExchangeRate operator -(ExchangeRate left, ExchangeRate right)
    {
        var result = left.Value - right.Value;
        if (result <= 0)
            throw new InvalidOperationException("Exchange rate cannot be zero or negative.");
        return new(result);
    }

    public static ExchangeRate operator *(ExchangeRate left, decimal factor)
        => new(left.Value * factor);

    public static ExchangeRate operator /(ExchangeRate left, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException();
        return new(left.Value / divisor);
    }

    public static bool operator >(ExchangeRate left, ExchangeRate right)
        => left.Value > right.Value;

    public static bool operator <(ExchangeRate left, ExchangeRate right)
        => left.Value < right.Value;

    public static bool operator >=(ExchangeRate left, ExchangeRate right)
        => left.Value >= right.Value;

    public static bool operator <=(ExchangeRate left, ExchangeRate right)
        => left.Value <= right.Value;

    public static implicit operator decimal(ExchangeRate rate)
        => rate.Value;

    public static explicit operator ExchangeRate(decimal value)
        => new(value);

    public ExchangeRate Abs() => new(decimal.Abs(Value));

    public ExchangeRate Round(int decimals = 6)
        => new(decimal.Round(Value, decimals, MidpointRounding.AwayFromZero));

    public static ExchangeRate Max(ExchangeRate left, ExchangeRate right)
        => left >= right ? left : right;

    public static ExchangeRate Min(ExchangeRate left, ExchangeRate right)
        => left <= right ? left : right;
}