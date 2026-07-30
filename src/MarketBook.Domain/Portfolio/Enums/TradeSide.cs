// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.Enums
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Portfolio.Enums;

/// <summary>
/// EN: Represents the side of a trade (Buy or Sell).
/// FA: جهت معامله (خرید یا فروش) را نمایش می‌دهد.
/// </summary>
public enum TradeSide
{
    /// <summary>
    /// EN: Buy trade (opening a long position).
    /// FA: معامله خرید (باز کردن موقعیت خرید).
    /// </summary>
    Buy = 1,

    /// <summary>
    /// EN: Sell trade (closing a long position or opening a short position).
    /// FA: معامله فروش (بستن موقعیت خرید یا باز کردن موقعیت فروش).
    /// </summary>
    Sell = 2
}