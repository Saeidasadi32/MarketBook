// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Industry.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;

namespace MarketBook.Domain.Industry.ValueObjects;

/// <summary>
/// EN: Represents the unique code of an industry.
/// FA: کد یکتای صنعت را نمایش می‌دهد.
/// </summary>
public sealed class IndustryCode : ValueObject
{
    /// <summary>
    /// EN: Maximum allowed length.
    /// FA: حداکثر طول مجاز.
    /// </summary>
    public const int MaxLength = 20;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="IndustryCode"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="IndustryCode"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Industry code.
    /// FA: کد صنعت.
    /// </param>
    public IndustryCode(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value);

        value = value.Trim().ToUpperInvariant();

        if (value.Length > MaxLength)
        {
            throw new DomainException(
                new Error(
                    $"{nameof(IndustryCode)}.TooLong",
                    $"Industry code cannot exceed {MaxLength} characters."));
        }

        Value = value;
    }

    /// <summary>
    /// EN: Gets the industry code.
    /// FA: کد صنعت را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc/>
    public override string ToString()
        => Value;

    /// <summary>
    /// EN: Implicit conversion to string.
    /// FA: تبدیل ضمنی به رشته.
    /// </summary>
    public static implicit operator string(IndustryCode value)
    {
        Guard.AgainstNull(value, nameof(value));

        return value.Value;
    }

    /// <summary>
    /// EN: Explicit conversion from string.
    /// FA: تبدیل صریح از رشته.
    /// </summary>
    public static explicit operator IndustryCode(string value)
        => new(value);
}
