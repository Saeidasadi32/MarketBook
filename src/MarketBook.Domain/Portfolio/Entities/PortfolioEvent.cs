// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.Entities
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.Financial.ValueObjects;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;

namespace MarketBook.Domain.Portfolio.Entities;

/// <summary>
/// EN: Represents a portfolio transaction.
/// FA: یک عملیات مالی در پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed class PortfolioEvent : Entity
{
    public PortfolioEvent(
        PortfolioEventId id,
        ListingId listingId,
        PortfolioEventType type,
        Quantity quantity,
        Money price,
        TransactionCost cost,
        DateTimeOffset executedOn)
    {
        Id = id;
        ListingId = listingId;
        Type = type;
        Quantity = quantity;
        Price = price;
        ExecutedOn = executedOn;
        Cost = cost;
    }

    /// <summary>
    /// EN: Transaction identifier.
    /// FA: شناسه معامله.
    /// </summary>
    public PortfolioEventId Id { get; }

    /// <summary>
    /// EN: Listing identifier.
    /// FA: شناسه نماد.
    /// </summary>
    public ListingId ListingId { get; }

    /// <summary>
    /// EN: Transaction type.
    /// FA: نوع عملیات.
    /// </summary>
    public PortfolioEventType Type { get; }

    /// <summary>
    /// EN: Quantity.
    /// FA: تعداد.
    /// </summary>
    public Quantity Quantity { get; }

    /// <summary>
    /// EN: Executed price.
    /// FA: قیمت اجرا.
    /// </summary>
    public Money Price { get; }

    /// <summary>
    /// EN: Execution time.
    /// FA: زمان انجام.
    /// </summary>
    public DateTimeOffset ExecutedOn { get; }

    /// <summary>
    /// EN: Gets total transaction value.
    /// FA: ارزش کل معامله.
    /// </summary>
    public Money GrossValue
        => new Money(Quantity.Value * Price.Value);

    /// <summary>
    /// EN: Gets transaction costs.
    /// FA: هزینه‌های معامله.
    /// </summary>
    public TransactionCost Cost { get; }

    public Money NetValue
    => GrossValue + Cost.Total;
}