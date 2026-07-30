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
    /// EN: Initializes a new instance of the <see cref="ListingId"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="ListingId"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">Underlying ULID value.</param>
    private ListingId(Ulid value) : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new unique identifier.
    /// FA: یک شناسه یکتای جدید ایجاد می‌کند.
    /// </summary>
    public static ListingId New() => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates an identifier from an existing ULID.
    /// FA: یک شناسه از یک ULID موجود ایجاد می‌کند.
    /// </summary>
    public static ListingId FromUlid(Ulid value) => new(value);

    /// <summary>
    /// EN: Parses a ULID string into an identifier.
    /// FA: رشته ULID را به شناسه تبدیل می‌کند.
    /// </summary>
    public static ListingId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(ListingId));

        if (Ulid.TryParse(value, out var ulid))
            return new(ulid);

        throw new DomainException(
            new Error(
                $"{nameof(ListingId)}.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را به شناسه تبدیل کند.
    /// </summary>
    public static bool TryParse(string? value, out ListingId? result)
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
    public static explicit operator ListingId(string value) => Parse(value);

}