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
/// EN: Defines supported portfolio cost-basis calculation methods.
/// FA: روش‌های پشتیبانی‌شده برای محاسبه بهای تمام‌شده پرتفوی را تعریف می‌کند.
/// </summary>
public enum PortfolioCostBasisMethod
{
    /// <summary>
    /// EN: Uses moving weighted-average acquisition cost.
/// FA: از میانگین موزون متحرک بهای خرید استفاده می‌کند.
    /// </summary>
    WeightedAverage = 1
}
