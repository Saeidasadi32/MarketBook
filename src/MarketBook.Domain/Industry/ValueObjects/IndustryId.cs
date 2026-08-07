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
using NUlid;

namespace MarketBook.Domain.Industry.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of an industry.
/// FA: شناسه یکتای یک صنعت را نمایش می‌دهد.
/// </summary>
public sealed record IndustryId : EntityId
{
    private IndustryId(Ulid value) : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new unique identifier.
    /// FA: یک شناسه یکتای جدید ایجاد می‌کند.
    /// </summary>
    public static IndustryId New() => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates an identifier from an existing ULID.
    /// FA: یک شناسه از یک ULID موجود ایجاد می‌کند.
    /// </summary>
    public static IndustryId FromUlid(Ulid value) => new(value);

    /// <summary>
    /// EN: Parses a ULID string.
    /// FA: رشته ULID را به شناسه تبدیل می‌کند.
    /// </summary>
    public static IndustryId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(IndustryId));

        if (Ulid.TryParse(value, out Ulid ulid))
            return new(ulid);

        throw new DomainException(
            new Error(
                "IndustryId.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را به شناسه تبدیل کند.
    /// </summary>
    public static bool TryParse(string? value, out IndustryId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) && Ulid.TryParse(value, out Ulid ulid))
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
    public static explicit operator IndustryId(string value) => Parse(value);
}
