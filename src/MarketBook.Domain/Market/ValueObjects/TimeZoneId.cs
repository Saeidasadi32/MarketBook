// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Market.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Instrument.ValueObjects;

namespace MarketBook.Domain.Market.ValueObjects;

/// <summary>
/// EN: Represents an IANA or Windows time zone identifier.
/// FA: شناسه منطقه زمانی (IANA یا Windows) را نمایش می‌دهد.
/// </summary>
public sealed record TimeZoneId
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="TimeZoneId"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="TimeZoneId"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Time zone identifier.
    /// FA: شناسه منطقه زمانی.
    /// </param>
    public TimeZoneId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value.Trim();
    }

    /// <summary>
    /// EN: Gets the time zone identifier.
    /// FA: شناسه منطقه زمانی را دریافت می‌کند.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// EN: Converts the value object to string.
    /// FA: مقدار شیء را به رشته تبدیل می‌کند.
    /// </summary>
    public string ToStringValue()
        => Value;

    /// <summary>
    /// EN: Creates asset class from string.
    /// FA: کلاس دارایی را از رشته ایجاد می‌کند.
    /// </summary>
    public static TimeZoneId FromString(string value)
        => new(value);

    /// <inheritdoc />
    public override string ToString() => Value;

    public static implicit operator string(TimeZoneId value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Value;
    }
}
