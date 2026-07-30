// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.CorporateActions.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.CorporateActions.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a corporate action.
/// FA: شناسه یکتای یک رویداد شرکتی را نمایش می‌دهد.
/// </summary>
public sealed record CorporateActionId : EntityId
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CorporateActionId"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="CorporateActionId"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">Underlying ULID value.</param>
    private CorporateActionId(Ulid value) : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new unique corporate action identifier.
    /// FA: یک شناسه یکتای جدید برای رویداد شرکتی ایجاد می‌کند.
    /// </summary>
    public static CorporateActionId New() => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates a corporate action identifier from an existing ULID.
    /// FA: یک شناسه رویداد شرکتی از یک ULID موجود ایجاد می‌کند.
    /// </summary>
    public static CorporateActionId FromUlid(Ulid value) => new(value);

    /// <summary>
    /// EN: Parses a ULID string into an identifier.
    /// FA: رشته ULID را به شناسه تبدیل می‌کند.
    /// </summary>
    public static CorporateActionId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value);

        if (Ulid.TryParse(value, out var ulid))
            return new(ulid);

        throw new DomainException(
            new Error(
                $"{nameof(CorporateActionId)}.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را به شناسه تبدیل کند.
    /// </summary>
    public static bool TryParse(string? value, out CorporateActionId? result)
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
    /// EN: explicit conversion to string.
    /// FA: تبدیل صریح به رشته.
    /// </summary>
    public static explicit operator CorporateActionId(string value) => Parse(value);

}