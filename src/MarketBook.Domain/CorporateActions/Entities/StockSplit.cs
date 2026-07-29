// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.CorporateActions.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.CorporateActions.Enums;
using MarketBook.Domain.CorporateActions.ValueObjects;

namespace MarketBook.Domain.CorporateActions.Entities;

/// <summary>
/// EN: Represents a stock split corporate action.
/// FA: رویداد تجزیه سهام را نمایش می‌دهد.
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
        int numerator,
        int denominator,
        string? description = null)
        : base(
            id,
            CorporateActionType.StockSplit,
            effectiveDate,
            description)
    {
        if (numerator <= 0)
            throw new ArgumentOutOfRangeException(nameof(numerator));

        if (denominator <= 0)
            throw new ArgumentOutOfRangeException(nameof(denominator));

        Numerator = numerator;
        Denominator = denominator;
    }

    /// <summary>
    /// EN: Gets split numerator.
    /// FA: صورت نسبت تجزیه را دریافت می‌کند.
    /// </summary>
    public int Numerator { get; }

    /// <summary>
    /// EN: Gets split denominator.
    /// FA: مخرج نسبت تجزیه را دریافت می‌کند.
    /// </summary>
    public int Denominator { get; }

    /// <summary>
    /// EN: Gets split ratio.
    /// FA: نسبت تجزیه را دریافت می‌کند.
    /// </summary>
    public decimal Ratio => (decimal)Numerator / Denominator;
}