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
/// EN: Represents an immutable monetary amount.
/// FA: یک مقدار پولی غیرقابل تغییر را نمایش می‌دهد.
/// </summary>
public readonly record struct Money :
    IComparable<Money>,
    IComparable
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Money"/> struct.
    /// FA: یک نمونه جدید از <see cref="Money"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Monetary amount.
    /// FA: مقدار پول.
    /// </param>
    public Money(decimal value)
    {
        Value = decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// EN: Gets monetary value.
    /// FA: مقدار پول را دریافت می‌کند.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// EN: Represents zero money.
    /// FA: مقدار صفر.
    /// </summary>
    public static Money Zero => new(0m);

    public bool IsZero => Value == 0m;

    public bool IsGreaterThanZero => Value > 0m;

    public bool IsPositive => Value > 0m;

    public bool IsNegative => Value < 0m;

    public int CompareTo(Money other)
        => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is Money other)
            return CompareTo(other);

        throw new ArgumentException(
            $"Object must be of type {nameof(Money)}.");
    }

    public override string ToString()
        => Value.ToString("N2");

    public static Money operator +(Money left, Money right)
        => new(left.Value + right.Value);

    public static Money operator -(Money left, Money right)
        => new(left.Value - right.Value);

    public static Money operator *(Money money, decimal factor)
        => new(money.Value * factor);

    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException();

        return new Money(money.Value / divisor);
    }

    public static Money Max(Money left, Money right)
    => left >= right ? left : right;

    public static Money Min(Money left, Money right)
        => left <= right ? left : right;

    public static implicit operator decimal(Money money)
        => money.Value;

    public static explicit operator Money(decimal value)
        => new(value);

    public Money Abs() => new(decimal.Abs(Value));

    public Money Round(int decimals = 2)
        => new(decimal.Round(
            Value,
            decimals,
            MidpointRounding.AwayFromZero));

    public static Money Parse(string value)
        => new(decimal.Parse(
            value,
            CultureInfo.InvariantCulture));

    public static bool TryParse(string value, out Money money)
    {
        if (decimal.TryParse(
            value,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out decimal result))
        {
            money = new(result);
            return true;
        }

        money = Zero;
        return false;
    }

}
