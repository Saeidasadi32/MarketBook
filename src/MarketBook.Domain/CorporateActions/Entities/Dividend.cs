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
using MarketBook.Domain.Financial.ValueObjects;

namespace MarketBook.Domain.CorporateActions.Entities;

/// <summary>
/// EN: Represents a dividend distribution.
/// FA: توزیع سود نقدی را نمایش می‌دهد.
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
        Money amountPerShare,
        string? description = null)
        : base(id, CorporateActionType.Dividend, effectiveDate, description)
    {
        if (amountPerShare.Value <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amountPerShare),
                "Dividend amount per share must be positive.");

        AmountPerShare = amountPerShare;
    }

    /// <summary>
    /// EN: Gets the dividend amount per share.
    /// FA: مبلغ سود نقدی به ازای هر سهم را دریافت می‌کند.
    /// </summary>
    public Money AmountPerShare { get; }

    /// <inheritdoc />
    public override Price Apply(Price price)
    {
        var adjustedPrice = price.Value - AmountPerShare.Value;
        if (adjustedPrice < 0)
            adjustedPrice = 0;

        MarkAsApplied();
        return new Price(adjustedPrice);
    }

    /// <inheritdoc />
    public override string ToString()
        => $"{base.ToString()} - {AmountPerShare} per share";
}