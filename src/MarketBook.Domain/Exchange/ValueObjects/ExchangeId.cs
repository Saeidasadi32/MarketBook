// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Exchange.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.Exchange.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a Exchange.
/// FA: شناسه یکتای Exchange را نمایش می‌دهد.
/// </summary>
public sealed record ExchangeId : EntityId
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="ExchangeId"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="ExchangeId"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">Underlying ULID value.</param>
    private ExchangeId(Ulid value) : base(value)
    {
}

/// <summary>
/// EN: Creates a new unique identifier.
/// FA: یک شناسه یکتای جدید ایجاد می‌کند.
/// </summary>
public static ExchangeId New () => new(Ulid.NewUlid());

/// <summary>
/// EN: Creates an identifier from an existing ULID.
/// FA: یک شناسه از یک ULID موجود ایجاد می‌کند.
/// </summary>
public static ExchangeId FromUlid (Ulid value) => new(value);

    /// <summary>
    /// EN: Parses a ULID string into an identifier.
    /// FA: رشته ULID را به شناسه تبدیل می‌کند.
    /// </summary>
    public static ExchangeId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(ExchangeId));

        if (Ulid.TryParse(value, out Ulid ulid))
            return new(ulid);

        throw new DomainException(
            new Error(
                $"{nameof(ExchangeId)}.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را به شناسه تبدیل کند.
    /// </summary>
    public static bool TryParse(string? value, out ExchangeId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) && Ulid.TryParse(value, out var ulid))
        {
            result = new(ulid);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// EN: Explicit conversion from string.
    /// FA: تبدیل صریح از رشته.
    /// </summary>
    public static explicit operator ExchangeId(string value) => Parse(value);

}
