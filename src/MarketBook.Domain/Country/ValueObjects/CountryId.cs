// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Country.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.Country.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a country.
/// FA: شناسه یکتای یک کشور را نمایش می‌دهد.
/// </summary>
public sealed record CountryId : EntityId
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CountryId"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="CountryId"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Underlying ULID value.
    /// FA: مقدار ULID.
    /// </param>
    private CountryId(Ulid value)
        : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new country identifier.
    /// FA: یک شناسه جدید برای کشور ایجاد می‌کند.
    /// </summary>
    /// <returns>
    /// EN: A new <see cref="CountryId"/>.
    /// FA: یک <see cref="CountryId"/> جدید.
    /// </returns>
    public static CountryId New()
        => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates a country identifier from an existing ULID.
    /// FA: یک شناسه کشور از یک ULID موجود ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Existing ULID value.
    /// FA: مقدار ULID موجود.
    /// </param>
    /// <returns>
    /// EN: A <see cref="CountryId"/>.
    /// FA: نمونه‌ای از <see cref="CountryId"/>.
    /// </returns>
    public static CountryId FromUlid(Ulid value)
        => new(value);

    /// <summary>
    /// EN: Parses a ULID string into a <see cref="CountryId"/>.
    /// FA: رشته ULID را به <see cref="CountryId"/> تبدیل می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: ULID string.
    /// FA: رشته ULID.
    /// </param>
    /// <returns>
    /// EN: Parsed <see cref="CountryId"/>.
    /// FA: شناسه ایجاد شده.
    /// </returns>
    public static CountryId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value);

        if (Ulid.TryParse(value, out var ulid))
        {
            return new(ulid);
        }

        throw new DomainException(new Error(
            $"{nameof(CountryId)}.InvalidFormat",
            $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را به شناسه تبدیل کند.
    /// </summary>
    /// <param name="value">
    /// EN: ULID string.
    /// FA: رشته ULID.
    /// </param>
    /// <param name="result">
    /// EN: Parsed identifier.
    /// FA: شناسه ایجاد شده.
    /// </param>
    /// <returns>
    /// EN: True if parsing succeeds.
    /// FA: در صورت موفقیت true.
    /// </returns>
    public static bool TryParse(string? value, out CountryId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Ulid.TryParse(value, out var ulid))
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
    public static explicit operator CountryId(string value) => Parse(value);
}