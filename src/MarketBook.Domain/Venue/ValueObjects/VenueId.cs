// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Venue.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.Venue.ValueObjects;

/// <summary>
/// EN: Represents the strongly typed unique identifier of a trading venue.
/// FA: شناسه یکتای Strongly Typed یک بستر معاملاتی را نمایش می‌دهد.
/// </summary>
public sealed record VenueId : EntityId
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="VenueId"/> class.
    /// FA: یک نمونه جدید از <see cref="VenueId"/> ایجاد می‌کند.
    /// </summary>
    private VenueId(Ulid value)
        : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new venue identifier.
    /// FA: یک شناسه جدید برای بستر معاملاتی ایجاد می‌کند.
    /// </summary>
    public static VenueId New()
        => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates a venue identifier from an existing ULID.
    /// FA: یک شناسه بستر معاملاتی را از ULID موجود ایجاد می‌کند.
    /// </summary>
    public static VenueId FromUlid(Ulid value)
        => new(value);

    /// <summary>
    /// EN: Parses a ULID string into a venue identifier.
    /// FA: یک رشته ULID را به شناسه بستر معاملاتی تبدیل می‌کند.
    /// </summary>
    public static VenueId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value);

        if (Ulid.TryParse(value, out Ulid ulid))
            return new(ulid);

        throw new DomainException(
            new Error(
                $"{nameof(VenueId)}.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string into a venue identifier.
    /// FA: تلاش می‌کند یک رشته ULID را به شناسه بستر معاملاتی تبدیل کند.
    /// </summary>
    public static bool TryParse(
        string? value,
        out VenueId? result)
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
    /// EN: Explicitly converts a string to a venue identifier.
    /// FA: یک رشته را به‌صورت صریح به شناسه بستر معاملاتی تبدیل می‌کند.
    /// </summary>
    public static explicit operator VenueId(string value)
        => Parse(value);
}
