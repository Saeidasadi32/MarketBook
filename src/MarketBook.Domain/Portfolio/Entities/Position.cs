// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.Financial.ValueObjects;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;

namespace MarketBook.Domain.Portfolio.Entities;

/// <summary>
/// EN: Represents an open investment position.
/// FA: یک موقعیت باز سرمایه‌گذاری را نمایش می‌دهد.
/// </summary>
public sealed class Position : Entity
{
    public Position(
        PositionId id,
        ListingId listingId,
        PositionSide side,
        Quantity quantity,
        Money averagePrice)
    {
        ArgumentNullException.ThrowIfNull(averagePrice);

        Id = id;
        ListingId = listingId;
        Side = side;
        Quantity = quantity;
        AveragePrice = averagePrice;
    }

    /// <summary>
    /// EN: Gets position identifier.
    /// FA: شناسه موقعیت.
    /// </summary>
    public PositionId Id { get; }

    /// <summary>
    /// EN: Gets listing identifier.
    /// FA: شناسه نماد.
    /// </summary>
    public ListingId ListingId { get; }

    /// <summary>
    /// EN: Gets position side.
    /// FA: نوع موقعیت.
    /// </summary>
    public PositionSide Side { get; }

    /// <summary>
    /// EN: Gets current quantity.
    /// FA: تعداد.
    /// </summary>
    public Quantity Quantity { get; private set; }

    /// <summary>
    /// EN: Gets average acquisition price.
    /// FA: میانگین قیمت خرید.
    /// </summary>
    public Money AveragePrice { get; private set; }

    /// <summary>
    /// EN: Applies a portfolio event to this position.
    /// FA: یک رویداد پرتفوی را روی این موقعیت اعمال می‌کند.
    /// </summary>
    public void Apply(PortfolioEvent portfolioEvent)
    {
        ArgumentNullException.ThrowIfNull(portfolioEvent);

        if (portfolioEvent.ListingId != ListingId)
            throw new InvalidOperationException(
                "Portfolio event does not belong to this position.");

        switch (portfolioEvent.Type)
        {
            case PortfolioEventType.Buy:
                ApplyBuy(portfolioEvent);
                break;

            case PortfolioEventType.Sell:
                ApplySell(portfolioEvent);
                break;

            default:
                throw new NotSupportedException(
                    $"Portfolio event '{portfolioEvent.Type}' is not supported yet.");
        }
    }

    /// <summary>
    /// EN: Applies a buy transaction.
    /// FA: خرید را اعمال می‌کند.
    /// </summary>
    private void ApplyBuy(PortfolioEvent portfolioEvent)
    {
        var totalCost =
            (AveragePrice.Value * Quantity.Value) +
            (portfolioEvent.Price.Value * portfolioEvent.Quantity.Value);

        var totalQuantity =
            Quantity.Value + portfolioEvent.Quantity.Value;

        Quantity = new Quantity(totalQuantity);

        AveragePrice = new Money(totalCost / totalQuantity);
    }

    /// <summary>
    /// EN: Applies a sell transaction.
    /// FA: فروش را اعمال می‌کند.
    /// </summary>
    private void ApplySell(PortfolioEvent portfolioEvent)
    {
        if (portfolioEvent.Quantity.Value > Quantity.Value)
            throw new InvalidOperationException(
                "Insufficient quantity.");

        Quantity = new Quantity(
            Quantity.Value - portfolioEvent.Quantity.Value);

        if (Quantity.IsZero)
        {
            AveragePrice = Money.Zero;
        }
    }
}