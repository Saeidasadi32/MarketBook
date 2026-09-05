// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Instrument.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.Instrument.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of an Instrument.
/// FA: شناسه یکتای Instrument را نمایش می‌دهد.
/// </summary>
public sealed record InstrumentId : EntityId
{
    /// <summary>
    /// EN: Initializes a new instrument identifier.
    /// FA: یک شناسه جدید برای ابزار مالی ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Underlying ULID value.
    /// FA: مقدار ULID زیرساختی.
    /// </param>
    private InstrumentId(Ulid value)
        : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new unique identifier.
    /// FA: یک شناسه یکتای جدید ایجاد می‌کند.
    /// </summary>
    public static InstrumentId New()
        => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates an identifier from an existing ULID.
    /// FA: یک شناسه از یک ULID موجود ایجاد می‌کند.
    /// </summary>
    public static InstrumentId FromUlid(Ulid value)
        => new(value);

    /// <summary>
    /// EN: Parses a ULID string into an identifier.
    /// FA: رشته ULID را به شناسه تبدیل می‌کند.
    /// </summary>
    public static InstrumentId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(InstrumentId));

        if (Ulid.TryParse(value, out Ulid ulid))
        {
            return new InstrumentId(ulid);
        }

        throw new DomainException(
            new Error(
                $"{nameof(InstrumentId)}.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را به شناسه تبدیل کند.
    /// </summary>
    public static bool TryParse(
        string? value,
        out InstrumentId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Ulid.TryParse(value, out Ulid ulid))
        {
            result = new InstrumentId(ulid);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// EN: Explicit conversion from string.
    /// FA: تبدیل صریح از رشته.
    /// </summary>
    public static explicit operator InstrumentId(string value)
        => Parse(value);
}
