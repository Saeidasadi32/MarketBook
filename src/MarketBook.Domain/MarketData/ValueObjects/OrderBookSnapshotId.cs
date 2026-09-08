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

/// <summary>
/// EN: Identifies one persisted order-book snapshot.
/// FA: یک Snapshot ذخیره‌شده از دفتر سفارشات را به‌صورت یکتا شناسایی می‌کند.
/// </summary>
public sealed record OrderBookSnapshotId : EntityId
{
    private OrderBookSnapshotId(Ulid value)
        : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new identifier.
    /// FA: یک شناسه جدید ایجاد می‌کند.
    /// </summary>
    public static OrderBookSnapshotId New()
        => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Parses an identifier from text.
    /// FA: شناسه را از متن تبدیل می‌کند.
    /// </summary>
    public static OrderBookSnapshotId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(OrderBookSnapshotId));

        if (Ulid.TryParse(value, out Ulid ulid))
            return new OrderBookSnapshotId(ulid);

        throw new DomainException(
            new Error(
                "OrderBookSnapshotId.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse an identifier from text.
    /// FA: تلاش می‌کند شناسه را از متن تبدیل کند.
    /// </summary>
    public static bool TryParse(string? value, out OrderBookSnapshotId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Ulid.TryParse(value, out Ulid ulid))
        {
            result = new OrderBookSnapshotId(ulid);
            return true;
        }

        result = null;
        return false;
    }
}
