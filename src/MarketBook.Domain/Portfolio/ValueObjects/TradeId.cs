// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.Portfolio.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a trade.
/// FA: شناسه یکتای یک معامله را نمایش می‌دهد.
/// </summary>
public sealed record TradeId : EntityId
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="TradeId"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="TradeId"/> را ایجاد می‌کند.
    /// </summary>
    private TradeId(Ulid value) : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new unique trade identifier.
    /// FA: یک شناسه یکتای جدید برای معامله ایجاد می‌کند.
    /// </summary>
    public static TradeId New() => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates a trade identifier from an existing ULID.
    /// FA: یک شناسه معامله از یک ULID موجود ایجاد می‌کند.
    /// </summary>
    public static TradeId FromUlid(Ulid value) => new(value);

    /// <summary>
    /// EN: Parses a ULID string into an identifier.
    /// FA: رشته ULID را به شناسه تبدیل می‌کند.
    /// </summary>
    public static TradeId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(TradeId));

        if (Ulid.TryParse(value, out var ulid))
            return new(ulid);

        throw new DomainException(
            new Error(
                $"{nameof(TradeId)}.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را به شناسه تبدیل کند.
    /// </summary>
    public static bool TryParse(string? value, out TradeId? result)
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
    public static explicit operator TradeId(string value) => Parse(value);
}