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
/// EN: Represents a portfolio transaction (Buy/Sell event).
/// FA: یک عملیات مالی در پرتفوی (خرید/فروش) را نمایش می‌دهد.
/// </summary>
public sealed class PortfolioEvent : Entity<PortfolioEventId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="PortfolioEvent"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="PortfolioEvent"/> را ایجاد می‌کند.
    /// </summary>
    public PortfolioEvent(
        PortfolioEventId id,
        ListingId listingId,
        PortfolioEventType type,
        Quantity quantity,
        Money price,
        TransactionCost cost,
        DateTimeOffset executedOn)
        : base(id)
    {
        ListingId = listingId;
        Type = type;
        Quantity = quantity;
        Price = price;
        ExecutedOn = executedOn;
        Cost = cost;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private PortfolioEvent()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets listing identifier.
    /// FA: شناسه نماد را دریافت می‌کند.
    /// </summary>
    public ListingId ListingId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets transaction type (Buy/Sell).
    /// FA: نوع عملیات (خرید/فروش) را دریافت می‌کند.
    /// </summary>
    public PortfolioEventType Type { get; }

    /// <summary>
    /// EN: Gets quantity.
    /// FA: تعداد را دریافت می‌کند.
    /// </summary>
    public Quantity Quantity { get; }

    /// <summary>
    /// EN: Gets executed price.
    /// FA: قیمت اجرا را دریافت می‌کند.
    /// </summary>
    public Money Price { get; }

    /// <summary>
    /// EN: Gets execution time.
    /// FA: زمان انجام را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset ExecutedOn { get; }

    /// <summary>
    /// EN: Gets transaction costs.
    /// FA: هزینه‌های معامله را دریافت می‌کند.
    /// </summary>
    public TransactionCost Cost { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the gross transaction value.
    /// FA: ارزش ناخالص معامله را دریافت می‌کند.
    /// </summary>
    public Money GrossValue => new(Quantity.Value * Price.Value);

    /// <summary>
    /// EN: Gets the net transaction value (gross + costs).
    /// FA: ارزش خالص معامله (ناخالص + هزینه‌ها) را دریافت می‌کند.
    /// </summary>
    public Money NetValue => GrossValue + Cost.Total;

    /// <summary>
    /// EN: Returns a string representation of the portfolio event.
    /// FA: نمایش رشته‌ای از رویداد پرتفوی را برمی‌گرداند.
    /// </summary>
    public override string ToString()
    {
        return $"{Type} {Quantity} @ {Price} on {ExecutedOn:yyyy-MM-dd HH:mm:ss}";
    }
}
