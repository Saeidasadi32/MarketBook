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
/// EN: Represents a cash dividend corporate action.
/// FA: رویداد سود نقدی را نمایش می‌دهد.
/// </summary>
public sealed class Dividend : CorporateAction
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Dividend"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Dividend"/> را ایجاد می‌کند.
    /// </summary>
    public Dividend(
        CorporateActionId id,
        DateOnly effectiveDate,
        Price dividendPerShare,
        string? description = null)
        : base(
            id,
            CorporateActionType.Dividend,
            effectiveDate,
            description)
    {
        DividendPerShare = dividendPerShare;
    }

    /// <summary>
    /// EN: Gets dividend amount per share.
    /// FA: مبلغ سود به ازای هر سهم را دریافت می‌کند.
    /// </summary>
    public Price DividendPerShare { get; }
}