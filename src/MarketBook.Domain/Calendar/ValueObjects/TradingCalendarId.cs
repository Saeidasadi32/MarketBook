// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Calendar.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.Calendar.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a trading calendar.
/// FA: شناسه یکتای تقویم معاملاتی را نمایش می‌دهد.
/// </summary>
public sealed record TradingCalendarId : EntityId
{
    private TradingCalendarId(Ulid value) : base(value)
    {
    }

    public static TradingCalendarId New() => new(Ulid.NewUlid());
    public static TradingCalendarId FromUlid(Ulid value) => new(value);

    public static TradingCalendarId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value);

        if (Ulid.TryParse(value, out var ulid))
            return new(ulid);

        throw new DomainException(
            new Error(
                "TradingCalendarId.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    public static bool TryParse(string? value, out TradingCalendarId? result)
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
    public static explicit operator TradingCalendarId(string value) => Parse(value);
}