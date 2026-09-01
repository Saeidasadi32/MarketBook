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
/// EN: Represents the strongly typed unique identifier of an exchange.
/// FA: شناسه یکتای Strongly Typed یک بورس یا بستر معاملاتی را نمایش می‌دهد.
/// </summary>
public sealed record ExchangeId : EntityId
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="ExchangeId"/> class.
    /// FA: یک نمونه جدید از <see cref="ExchangeId"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Underlying ULID value.
    /// FA: مقدار ULID پایه.
    /// </param>
    private ExchangeId(Ulid value)
        : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new unique exchange identifier.
    /// FA: یک شناسه یکتای جدید برای بورس ایجاد می‌کند.
    /// </summary>
    public static ExchangeId New()
        => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates an exchange identifier from an existing ULID.
    /// FA: یک شناسه بورس را از ULID موجود ایجاد می‌کند.
    /// </summary>
    public static ExchangeId FromUlid(Ulid value)
        => new(value);

    /// <summary>
    /// EN: Parses a ULID string into an exchange identifier.
    /// FA: یک رشته ULID را به شناسه بورس تبدیل می‌کند.
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
    /// EN: Attempts to parse a ULID string into an exchange identifier.
    /// FA: تلاش می‌کند یک رشته ULID را به شناسه بورس تبدیل کند.
    /// </summary>
    public static bool TryParse(
        string? value,
        out ExchangeId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Ulid.TryParse(value, out Ulid ulid))
        {
            result = new(ulid);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// EN: Explicitly converts a string to an exchange identifier.
    /// FA: یک رشته را به‌صورت صریح به شناسه بورس تبدیل می‌کند.
    /// </summary>
    public static explicit operator ExchangeId(string value)
        => Parse(value);
}
