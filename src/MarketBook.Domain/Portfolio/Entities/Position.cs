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
public sealed class Position : Entity<PositionId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Position"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Position"/> را ایجاد می‌کند.
    /// </summary>
    public Position(
        PositionId id,
        ListingId listingId,
        PositionSide side,
        Quantity quantity,
        Money averagePrice)
        : base(id)
    {
        ListingId = listingId;
        Side = side;
        Quantity = quantity;
        AveragePrice = averagePrice;
        CreatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Position()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets listing identifier.
    /// FA: شناسه نماد را دریافت می‌کند.
    /// </summary>
    public ListingId ListingId { get; }

    /// <summary>
    /// EN: Gets position side (Long/Short).
    /// FA: نوع موقعیت (خرید/فروش) را دریافت می‌کند.
    /// </summary>
    public PositionSide Side { get; }

    /// <summary>
    /// EN: Gets current quantity.
    /// FA: تعداد فعلی را دریافت می‌کند.
    /// </summary>
    public Quantity Quantity { get; private set; }

    /// <summary>
    /// EN: Gets average acquisition price.
    /// FA: میانگین قیمت خرید را دریافت می‌کند.
    /// </summary>
    public Money AveragePrice { get; private set; }

    /// <summary>
    /// EN: Gets the creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets the total cost of the position.
    /// FA: هزینه کل موقعیت را دریافت می‌کند.
    /// </summary>
    public Money TotalCost => new(Quantity.Value * AveragePrice.Value);

    /// <summary>
    /// EN: Gets the current market value of the position.
    /// FA: ارزش بازار فعلی موقعیت را دریافت می‌کند.
    /// </summary>
    public Money MarketValue { get; private set; }

    /// <summary>
    /// EN: Applies a portfolio event to this position.
    /// FA: یک رویداد پرتفوی را روی این موقعیت اعمال می‌کند.
    /// </summary>
    public void Apply(PortfolioEvent portfolioEvent)
    {
        ArgumentNullException.ThrowIfNull(portfolioEvent);

        if (portfolioEvent.ListingId != ListingId)
            throw new DomainException(
                new Error(
                    "Position.EventMismatch",
                    "Portfolio event does not belong to this position."));

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
                    $"Portfolio event '{portfolioEvent.Type}' is not supported.");
        }

        AddDomainEvent(new PositionUpdatedEvent(Id, Quantity, AveragePrice));
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

        var totalQuantity = Quantity.Value + portfolioEvent.Quantity.Value;

        if (totalQuantity == 0)
            throw new DomainException(
                new Error(
                    "Position.InvalidQuantity",
                    "Total quantity cannot be zero."));

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
            throw new DomainException(
                new Error(
                    "Position.InsufficientQuantity",
                    "Insufficient quantity to sell."));

        Quantity = new Quantity(Quantity.Value - portfolioEvent.Quantity.Value);

        if (Quantity.IsZero)
        {
            AveragePrice = Money.Zero;
        }
    }

    /// <summary>
    /// EN: Updates the market value of the position.
    /// FA: ارزش بازار موقعیت را به‌روزرسانی می‌کند.
    /// </summary>
    public void UpdateMarketValue(Price currentPrice)
    {
        MarketValue = new Money(Quantity.Value * currentPrice.Value);
    }

    /// <summary>
    /// EN: Calculates the unrealized profit or loss.
    /// FA: سود یا زیان تحقق‌نیافته را محاسبه می‌کند.
    /// </summary>
    public Money CalculateUnrealizedPnL()
    {
        return MarketValue - TotalCost;
    }
}

/// <summary>
/// EN: Domain event raised when a position is updated.
/// FA: رویداد دامنه زمانی که موقعیت به‌روزرسانی می‌شود.
/// </summary>
public sealed record PositionUpdatedEvent(
    PositionId PositionId,
    Quantity NewQuantity,
    Money NewAveragePrice) : DomainEvent;