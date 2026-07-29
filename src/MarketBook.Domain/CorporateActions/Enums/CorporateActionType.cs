// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.CorporateActions.Enums
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.CorporateActions.Enums;

/// <summary>
/// EN: Defines corporate action types.
/// FA: انواع رویدادهای شرکتی را مشخص می‌کند.
/// </summary>
public enum CorporateActionType
{
    /// <summary>
    /// EN: Cash dividend.
    /// FA: سود نقدی.
    /// </summary>
    Dividend = 1,

    /// <summary>
    /// EN: Capital increase.
    /// FA: افزایش سرمایه.
    /// </summary>
    CapitalIncrease = 2,

    /// <summary>
    /// EN: Stock split.
    /// FA: تجزیه سهام.
    /// </summary>
    StockSplit = 3,

    /// <summary>
    /// EN: Reverse split.
    /// FA: تجمیع سهام.
    /// </summary>
    ReverseSplit = 4,

    /// <summary>
    /// EN: Symbol changed.
    /// FA: تغییر نماد.
    /// </summary>
    SymbolChange = 5,

    /// <summary>
    /// EN: Trading suspension.
    /// FA: توقف معاملات.
    /// </summary>
    Suspension = 6,

    /// <summary>
    /// EN: Delisting.
    /// FA: حذف از بازار.
    /// </summary>
    Delisting = 7
}