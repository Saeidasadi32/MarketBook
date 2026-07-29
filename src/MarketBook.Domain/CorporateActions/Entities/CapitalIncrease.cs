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
/// EN: Represents a capital increase corporate action.
/// FA: رویداد افزایش سرمایه را نمایش می‌دهد.
/// </summary>
public sealed class CapitalIncrease : CorporateAction
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CapitalIncrease"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="CapitalIncrease"/> را ایجاد می‌کند.
    /// </summary>
    public CapitalIncrease(
        CorporateActionId id,
        DateOnly effectiveDate,
        decimal percentage,
        string source,
        string? description = null)
        : base(
            id,
            CorporateActionType.CapitalIncrease,
            effectiveDate,
            description)
    {
        if (percentage <= 0)
            throw new ArgumentOutOfRangeException(nameof(percentage));

        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        Percentage = percentage;
        Source = source.Trim();
    }

    /// <summary>
    /// EN: Gets increase percentage.
    /// FA: درصد افزایش سرمایه را دریافت می‌کند.
    /// </summary>
    public decimal Percentage { get; }

    /// <summary>
    /// EN: Gets increase source.
    /// FA: منبع افزایش سرمایه را دریافت می‌کند.
    /// </summary>
    public string Source { get; }
}