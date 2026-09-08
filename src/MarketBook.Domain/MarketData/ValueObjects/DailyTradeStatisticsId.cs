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
/// EN: Identifies a persisted daily trade-statistics record.
/// FA: رکورد آمار معاملات روزانه را به‌صورت یکتا شناسایی می‌کند.
/// </summary>
public sealed record DailyTradeStatisticsId : EntityId
{
    private DailyTradeStatisticsId(Ulid value)
        : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new identifier.
    /// FA: یک شناسه جدید ایجاد می‌کند.
    /// </summary>
    public static DailyTradeStatisticsId New()
        => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Parses an identifier from text.
    /// FA: شناسه را از متن تبدیل می‌کند.
    /// </summary>
    public static DailyTradeStatisticsId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(DailyTradeStatisticsId));

        if (Ulid.TryParse(value, out Ulid ulid))
            return new DailyTradeStatisticsId(ulid);

        throw new DomainException(
            new Error(
                "DailyTradeStatisticsId.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse an identifier from text.
    /// FA: تلاش می‌کند شناسه را از متن تبدیل کند.
    /// </summary>
    public static bool TryParse(
        string? value,
        out DailyTradeStatisticsId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Ulid.TryParse(value, out Ulid ulid))
        {
            result = new DailyTradeStatisticsId(ulid);
            return true;
        }

        result = null;
        return false;
    }
}
