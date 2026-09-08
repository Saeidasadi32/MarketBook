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
/// EN: Represents the unique identifier of a persisted portfolio transaction.
/// FA: شناسه یکتای تراکنش ماندگار پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed record PortfolioTransactionId : EntityId
{
    private PortfolioTransactionId(Ulid value) : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new unique transaction identifier.
    /// FA: یک شناسه یکتای جدید برای تراکنش ایجاد می‌کند.
    /// </summary>
    public static PortfolioTransactionId New() => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Parses a ULID string.
    /// FA: رشته ULID را تبدیل می‌کند.
    /// </summary>
    public static PortfolioTransactionId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(PortfolioTransactionId));

        if (Ulid.TryParse(value, out Ulid ulid))
            return new PortfolioTransactionId(ulid);

        throw new DomainException(
            new Error(
                "PortfolioTransactionId.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را تبدیل کند.
    /// </summary>
    public static bool TryParse(string? value, out PortfolioTransactionId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Ulid.TryParse(value, out Ulid ulid))
        {
            result = new PortfolioTransactionId(ulid);
            return true;
        }

        result = null;
        return false;
    }
}
