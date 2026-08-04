// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Currency.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.Currency.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a currency.
/// FA: شناسه یکتای یک ارز را نمایش می‌دهد.
/// </summary>
public sealed record CurrencyId : EntityId
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="[Entity]Id"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="[Entity]Id"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">Underlying ULID value.</param>
    private CurrencyId(Ulid value) : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new currency identifier.
    /// FA: یک شناسه جدید برای ارز ایجاد می‌کند.
    /// </summary>
    public static CurrencyId New() => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates an identifier from an existing ULID.
    /// FA: یک شناسه از یک ULID موجود ایجاد می‌کند.
    /// </summary>
    public static CurrencyId FromUlid (Ulid value) => new(value);

    /// <summary>
    /// EN: Parses a ULID string into an identifier.
    /// FA: رشته ULID را به شناسه تبدیل می‌کند.
    /// </summary>
    public static CurrencyId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value);

        if (Ulid.TryParse(value, out Ulid ulid))
            return new(ulid);

        throw new DomainException(
            new Error(
                $"{nameof(CurrencyId)}.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را به شناسه تبدیل کند.
    /// </summary>
    public static bool TryParse(string? value, out CurrencyId? result)
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
    public static explicit operator CurrencyId(string value) => Parse(value);
}
