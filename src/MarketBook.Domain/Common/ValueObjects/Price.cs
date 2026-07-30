// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Globalization;

namespace MarketBook.Domain.Common.ValueObjects;

/// <summary>
/// EN: Represents a monetary price with comparison and arithmetic operations.
/// FA: یک مقدار قیمت با عملیات مقایسه و حسابی را نمایش می‌دهد.
/// </summary>
public readonly record struct Price :
    IComparable<Price>,
    IComparable
{
    /// <summary>
    /// EN: Initializes a new price.
    /// FA: یک قیمت جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Price value.
    /// FA: مقدار قیمت.
    /// </param>
    public Price(decimal value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Price cannot be negative.");

        Value = decimal.Round(value, 6, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// EN: Gets price value.
    /// FA: مقدار قیمت را دریافت می‌کند.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// EN: Represents a zero price.
    /// FA: قیمت صفر را نمایش می‌دهد.
    /// </summary>
    public static Price Zero => new(0m);

    /// <summary>
    /// EN: Gets a value indicating whether the price is zero.
    /// FA: مشخص می‌کند قیمت صفر است یا خیر.
    /// </summary>
    public bool IsZero => Value == 0m;

    /// <summary>
    /// EN: Gets a value indicating whether the price is positive.
    /// FA: مشخص می‌کند قیمت مثبت است یا خیر.
    /// </summary>
    public bool IsPositive => Value > 0m;

    /// <summary>
    /// EN: Compares the current price with another price.
    /// FA: قیمت جاری را با قیمت دیگر مقایسه می‌کند.
    /// </summary>
    public int CompareTo(Price other)
        => Value.CompareTo(other.Value);

    /// <summary>
    /// EN: Compares the current price with another object.
    /// FA: قیمت جاری را با شیء دیگر مقایسه می‌کند.
    /// </summary>
    public int CompareTo(object? obj)
    {
        if (obj is Price other)
            return CompareTo(other);

        throw new ArgumentException($"Object must be of type {nameof(Price)}.");
    }

    /// <summary>
    /// EN: Returns the string representation of the price.
    /// FA: نمایش رشته‌ای قیمت را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => Value.ToString("0.######", CultureInfo.InvariantCulture);

    /// <summary>
    /// EN: Parses a string to a price.
    /// FA: یک رشته را به قیمت تبدیل می‌کند.
    /// </summary>
    public static Price Parse(string value)
        => new(decimal.Parse(value, CultureInfo.InvariantCulture));

    /// <summary>
    /// EN: Tries to parse a string to a price.
    /// FA: سعی می‌کند یک رشته را به قیمت تبدیل کند.
    /// </summary>
    public static bool TryParse(string value, out Price price)
    {
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
        {
            price = new(result);
            return true;
        }

        price = Zero;
        return false;
    }

    /// <summary>
    /// EN: Rounds the price to the specified number of decimals.
    /// FA: قیمت را به تعداد اعشار مشخص شده گرد می‌کند.
    /// </summary>
    public Price Round(int decimals = 6)
        => new(decimal.Round(Value, decimals, MidpointRounding.AwayFromZero));

    /// <summary>
    /// EN: Returns the absolute value of the price.
    /// FA: مقدار مطلق قیمت را برمی‌گرداند.
    /// </summary>
    public Price Abs() => new(decimal.Abs(Value));

    /// <summary>
    /// EN: Returns the maximum of two prices.
    /// FA: ماکزیمم دو قیمت را برمی‌گرداند.
    /// </summary>
    public static Price Max(Price left, Price right)
        => left >= right ? left : right;

    /// <summary>
    /// EN: Returns the minimum of two prices.
    /// FA: مینیمم دو قیمت را برمی‌گرداند.
    /// </summary>
    public static Price Min(Price left, Price right)
        => left <= right ? left : right;

    // Operators
    public static Price operator +(Price left, Price right)
        => new(left.Value + right.Value);

    public static Price operator -(Price left, Price right)
    {
        var result = left.Value - right.Value;
        if (result < 0)
            throw new InvalidOperationException("Price cannot be negative.");
        return new(result);
    }

    public static Price operator *(Price left, decimal factor)
        => new(left.Value * factor);

    public static Price operator /(Price left, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException();
        return new(left.Value / divisor);
    }

    public static bool operator >(Price left, Price right)
        => left.Value > right.Value;

    public static bool operator <(Price left, Price right)
        => left.Value < right.Value;

    public static bool operator >=(Price left, Price right)
        => left.Value >= right.Value;

    public static bool operator <=(Price left, Price right)
        => left.Value <= right.Value;

    public static implicit operator decimal(Price price)
        => price.Value;

    public static explicit operator Price(decimal value)
        => new(value);
}