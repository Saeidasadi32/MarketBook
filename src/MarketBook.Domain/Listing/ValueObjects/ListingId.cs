// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Listing.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.Listing.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a Listing.
/// FA: شناسه یکتای Listing را نمایش می‌دهد.
/// </summary>
public sealed record ListingId : EntityId
{
    /// <summary>
    /// EN: Initializes a listing identifier from an underlying ULID.
    /// FA: شناسه Listing را از یک ULID زیرساختی ایجاد می‌کند.
    /// </summary>
    private ListingId(Ulid value)
        : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new unique listing identifier.
    /// FA: یک شناسه یکتای جدید برای Listing ایجاد می‌کند.
    /// </summary>
    public static ListingId New()
        => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates a listing identifier from an existing ULID.
    /// FA: شناسه Listing را از یک ULID موجود ایجاد می‌کند.
    /// </summary>
    public static ListingId FromUlid(Ulid value)
        => new(value);

    /// <summary>
    /// EN: Parses a ULID string into a listing identifier.
    /// FA: رشته ULID را به شناسه Listing تبدیل می‌کند.
    /// </summary>
    public static ListingId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(ListingId));

        if (Ulid.TryParse(value, out Ulid ulid))
        {
            return new ListingId(ulid);
        }

        throw new DomainException(
            new Error(
                $"{nameof(ListingId)}.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را به شناسه Listing تبدیل کند.
    /// </summary>
    public static bool TryParse(
        string? value,
        out ListingId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Ulid.TryParse(value, out Ulid ulid))
        {
            result = new ListingId(ulid);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// EN: Explicitly converts a string into a listing identifier.
    /// FA: رشته را به‌صورت صریح به شناسه Listing تبدیل می‌کند.
    /// </summary>
    public static explicit operator ListingId(string value)
        => Parse(value);
}
