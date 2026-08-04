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
/// EN: Represents a completed trade (executed order).
/// FA: یک معامله انجام‌شده (سفارش اجرا شده) را نمایش می‌دهد.
/// </summary>
public sealed class Trade : Entity<TradeId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Trade"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Trade"/> را ایجاد می‌کند.
    /// </summary>
    public Trade(
        TradeId id,
        ListingId listingId,
        PortfolioId portfolioId,
        TradeSide side,
        Quantity quantity,
        Price price,
        Money commission,
        DateTimeOffset executedOn,
        string? reference = null)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(listingId);
        ArgumentNullException.ThrowIfNull(portfolioId);

        ListingId = listingId;
        PortfolioId = portfolioId;
        Side = side;
        Quantity = quantity;
        Price = price;
        Commission = commission;
        ExecutedOn = executedOn;
        Reference = reference?.Trim();
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Trade()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets the listing identifier (symbol).
    /// FA: شناسه نماد معاملاتی را دریافت می‌کند.
    /// </summary>
    public ListingId ListingId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the portfolio identifier.
    /// FA: شناسه پرتفوی را دریافت می‌کند.
    /// </summary>
    public PortfolioId PortfolioId { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the trade side (Buy/Sell).
    /// FA: جهت معامله (خرید/فروش) را دریافت می‌کند.
    /// </summary>
    public TradeSide Side { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the traded quantity.
    /// FA: حجم معامله را دریافت می‌کند.
    /// </summary>
    public Quantity Quantity { get; }

    /// <summary>
    /// EN: Gets the execution price.
    /// FA: قیمت اجرای معامله را دریافت می‌کند.
    /// </summary>
    public Price Price { get; }

    /// <summary>
    /// EN: Gets the commission paid for this trade.
    /// FA: کارمزد پرداخت شده برای این معامله را دریافت می‌کند.
    /// </summary>
    public Money Commission { get; }

    /// <summary>
    /// EN: Gets the execution timestamp.
    /// FA: زمان انجام معامله را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset ExecutedOn { get; }

    /// <summary>
    /// EN: Gets the reference number (optional).
    /// FA: شماره مرجع (اختیاری) را دریافت می‌کند.
    /// </summary>
    public string? Reference { get; }

    /// <summary>
    /// EN: Gets the gross trade value (Quantity × Price).
    /// FA: ارزش ناخالص معامله (حجم × قیمت) را دریافت می‌کند.
    /// </summary>
    public Money GrossValue => new(Quantity.Value * Price.Value);

    /// <summary>
    /// EN: Gets the net trade value (Gross - Commission).
    /// FA: ارزش خالص معامله (ناخالص - کارمزد) را دریافت می‌کند.
    /// </summary>
    public Money NetValue => new(GrossValue.Value - Commission.Value);

    /// <summary>
    /// EN: Gets a value indicating whether this is a buy trade.
    /// FA: مشخص می‌کند که این معامله خرید است یا خیر.
    /// </summary>
    public bool IsBuy => Side == TradeSide.Buy;

    /// <summary>
    /// EN: Gets a value indicating whether this is a sell trade.
    /// FA: مشخص می‌کند که این معامله فروش است یا خیر.
    /// </summary>
    public bool IsSell => Side == TradeSide.Sell;

    /// <summary>
    /// EN: Returns a string representation of the trade.
    /// FA: نمایش رشته‌ای از معامله را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => $"{Side} {Quantity} @ {Price} = {GrossValue} [{ExecutedOn:yyyy-MM-dd HH:mm:ss}]";

    /// <summary>
    /// EN: Determines whether the specified trade is equal to the current trade.
    /// FA: تعیین می‌کند که معامله مشخص شده با معامله جاری برابر است یا خیر.
    /// </summary>
    public bool Equals(Trade? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as Trade);

    /// <inheritdoc />
    public override int GetHashCode()
        => Id.GetHashCode();
}
