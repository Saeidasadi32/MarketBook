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
/// EN: Represents the unique identifier of a persisted portfolio cash transaction.
/// FA: شناسه یکتای تراکنش ماندگار دفتر نقدی پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed record PortfolioCashTransactionId : EntityId
{
    private PortfolioCashTransactionId(Ulid value) : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new unique cash-transaction identifier.
    /// FA: یک شناسه یکتای جدید برای تراکنش نقدی ایجاد می‌کند.
    /// </summary>
    public static PortfolioCashTransactionId New() => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Parses a ULID string.
    /// FA: رشته ULID را تبدیل می‌کند.
    /// </summary>
    public static PortfolioCashTransactionId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(PortfolioCashTransactionId));

        if (Ulid.TryParse(value, out Ulid ulid))
            return new PortfolioCashTransactionId(ulid);

        throw new DomainException(
            new Error(
                "PortfolioCashTransactionId.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را تبدیل کند.
    /// </summary>
    public static bool TryParse(string? value, out PortfolioCashTransactionId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) && Ulid.TryParse(value, out Ulid ulid))
        {
            result = new PortfolioCashTransactionId(ulid);
            return true;
        }

        result = null;
        return false;
    }
}
