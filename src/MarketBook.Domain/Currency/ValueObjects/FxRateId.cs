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
/// EN: Identifies a persisted FX-rate record.
/// FA: رکورد نرخ تبدیل ارز را به‌صورت یکتا شناسایی می‌کند.
/// </summary>
public sealed record FxRateId : EntityId
{
    private FxRateId(Ulid value)
        : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new FX-rate identifier.
    /// FA: یک شناسه جدید برای نرخ تبدیل ارز ایجاد می‌کند.
    /// </summary>
    public static FxRateId New() => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Parses an FX-rate identifier.
    /// FA: شناسه نرخ تبدیل ارز را از رشته تبدیل می‌کند.
    /// </summary>
    /// <param name="value">EN: Identifier text. FA: متن شناسه.</param>
    /// <returns>EN: Parsed identifier. FA: شناسه تبدیل‌شده.</returns>
    public static FxRateId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(FxRateId));

        if (Ulid.TryParse(value, out Ulid ulid))
        {
            return new FxRateId(ulid);
        }

        throw new DomainException(
            new Error(
                "FxRateId.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse an FX-rate identifier.
    /// FA: تلاش می‌کند شناسه نرخ تبدیل ارز را تبدیل کند.
    /// </summary>
    /// <param name="value">EN: Identifier text. FA: متن شناسه.</param>
    /// <param name="result">EN: Parsed identifier when successful. FA: شناسه تبدیل‌شده در صورت موفقیت.</param>
    /// <returns>EN: True when parsing succeeds. FA: در صورت موفقیت true.</returns>
    public static bool TryParse(string? value, out FxRateId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Ulid.TryParse(value, out Ulid ulid))
        {
            result = new FxRateId(ulid);
            return true;
        }

        result = null;
        return false;
    }
}
