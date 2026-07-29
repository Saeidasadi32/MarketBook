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

namespace MarketBook.Domain.Portfolio.Entities;

/// <summary>
/// EN: Represents a completed trade.
/// FA: یک معامله انجام‌شده را نمایش می‌دهد.
/// </summary>
public sealed class Trade : Entity
{
    /// <summary>
    /// EN: Initializes a new trade.
    /// FA: یک معامله جدید ایجاد می‌کند.
    /// </summary>
    public Trade(
        DateOnly tradeDate,
        string symbol,
        decimal quantity,
        decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price));

        TradeDate = tradeDate;
        Symbol = symbol.Trim().ToUpperInvariant();
        Quantity = quantity;
        Price = price;
    }

    /// <summary>
    /// EN: Gets trade date.
    /// FA: تاریخ معامله را دریافت می‌کند.
    /// </summary>
    public DateOnly TradeDate { get; }

    /// <summary>
    /// EN: Gets trading symbol.
    /// FA: نماد معاملاتی را دریافت می‌کند.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// EN: Gets traded quantity.
    /// FA: حجم معامله را دریافت می‌کند.
    /// </summary>
    public decimal Quantity { get; }

    /// <summary>
    /// EN: Gets execution price.
    /// FA: قیمت انجام معامله را دریافت می‌کند.
    /// </summary>
    public decimal Price { get; }

    /// <summary>
    /// EN: Gets trade value.
    /// FA: ارزش معامله را دریافت می‌کند.
    /// </summary>
    public decimal Value => Quantity * Price;
}