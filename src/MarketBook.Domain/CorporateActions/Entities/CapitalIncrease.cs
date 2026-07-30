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
/// EN: Represents a capital increase (rights issue).
/// FA: افزایش سرمایه (حق تقدم) را نمایش می‌دهد.
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
        decimal ratio,  // e.g., 0.5 means 1 new share for every 2 existing shares
        Money subscriptionPrice,
        string? description = null)
        : base(id, CorporateActionType.CapitalIncrease, effectiveDate, description)
    {
        if (ratio <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(ratio),
                "Ratio must be greater than zero.");

        Ratio = ratio;
        SubscriptionPrice = subscriptionPrice;
    }

    /// <summary>
    /// EN: Gets the capital increase ratio.
    /// FA: نسبت افزایش سرمایه را دریافت می‌کند.
    /// </summary>
    public decimal Ratio { get; }

    /// <summary>
    /// EN: Gets the subscription price per new share.
    /// FA: قیمت هر سهم جدید را دریافت می‌کند.
    /// </summary>
    public Money SubscriptionPrice { get; }

    /// <inheritdoc />
    public override Price Apply(Price price)
    {
        // Theoretical ex-rights price formula:
        // TERP = (Current Price + Ratio * Subscription Price) / (1 + Ratio)
        var term = (price.Value + (Ratio * SubscriptionPrice.Value)) / (1 + Ratio);

        MarkAsApplied();
        return new Price(term);
    }

    /// <inheritdoc />
    public override string ToString()
        => $"{base.ToString()} - Ratio: {Ratio}, Subscription Price: {SubscriptionPrice}";
}