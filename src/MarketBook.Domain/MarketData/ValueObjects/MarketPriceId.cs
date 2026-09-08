// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.MarketData.ValueObjects;
/// <summary>EN: Identifies a persisted market-price record. FA: رکورد قیمت بازار را به‌صورت یکتا شناسایی می‌کند.</summary>
public sealed record MarketPriceId : EntityId
{
    private MarketPriceId(Ulid value) : base(value) { }
/// <summary>EN: Creates a new identifier. FA: شناسه جدید ایجاد می‌کند.</summary>
public static MarketPriceId New() => new(Ulid.NewUlid());
/// <summary>EN: Parses an identifier. FA: شناسه را از رشته تبدیل می‌کند.</summary>
public static MarketPriceId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(MarketPriceId));
        if (Ulid.TryParse(value, out Ulid ulid)) return new MarketPriceId(ulid);
        throw new DomainException(new Error("MarketPriceId.InvalidFormat", $"'{value}' is not a valid ULID."));
    }
/// <summary>EN: Attempts to parse an identifier. FA: تلاش می‌کند شناسه را تبدیل کند.</summary>
public static bool TryParse(string? value, out MarketPriceId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) && Ulid.TryParse(value, out Ulid ulid)) { result = new MarketPriceId(ulid); return true; }
        result = null; return false;
    }
}
