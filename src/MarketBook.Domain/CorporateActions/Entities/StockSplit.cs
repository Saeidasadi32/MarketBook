// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.CorporateActions.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.CorporateActions.Enums;
using MarketBook.Domain.CorporateActions.ValueObjects;

namespace MarketBook.Domain.CorporateActions.Entities;

/// <summary>
/// EN: Represents a stock split (or reverse split).
/// FA: تقسیم سهام (یا معکوس) را نمایش می‌دهد.
/// </summary>
public sealed class StockSplit : CorporateAction
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="StockSplit"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="StockSplit"/> را ایجاد می‌کند.
    /// </summary>
    public StockSplit(
        CorporateActionId id,
        DateOnly effectiveDate,
        int splitFactor,
        bool isReverseSplit = false,
        string? description = null)
        : base(id, CorporateActionType.StockSplit, effectiveDate, description)
    {
        if (splitFactor <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(splitFactor),
                "Split factor must be positive.");

        SplitFactor = splitFactor;
        IsReverseSplit = isReverseSplit;
    }

    /// <summary>
    /// EN: Gets the split factor (e.g., 2 for a 2-for-1 split).
    /// FA: ضریب تقسیم (مثلاً ۲ برای تقسیم ۲ به ۱).
    /// </summary>
    public int SplitFactor { get; }

    /// <summary>
    /// EN: Gets a value indicating whether this is a reverse split.
    /// FA: مشخص می‌کند که این یک تقسیم معکوس است یا خیر.
    /// </summary>
    public bool IsReverseSplit { get; }

    /// <inheritdoc />
    public override Price Apply(Price price)
    {
        var factor = IsReverseSplit ? 1m / SplitFactor : SplitFactor;
        var adjustedPrice = price.Value * factor;

        MarkAsApplied();
        return new Price(adjustedPrice);
    }

    /// <inheritdoc />
    public override string ToString()
        => $"{base.ToString()} - {(IsReverseSplit ? "Reverse" : "Forward")} {SplitFactor}:1";
}