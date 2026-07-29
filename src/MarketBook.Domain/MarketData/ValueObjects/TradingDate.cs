// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.MarketData.ValueObjects;

/// <summary>
/// EN: Represents a trading date.
/// FA: تاریخ معاملاتی را نمایش می‌دهد.
/// </summary>
public readonly record struct TradingDate
{
    /// <summary>
    /// EN: Initializes a new trading date.
    /// FA: یک تاریخ معاملاتی جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Trading date.
    /// FA: تاریخ معامله.
    /// </param>
    public TradingDate(DateOnly value)
    {
        Value = value;
    }

    /// <summary>
    /// EN: Gets trading date.
    /// FA: تاریخ معاملاتی را دریافت می‌کند.
    /// </summary>
    public DateOnly Value { get; }

    /// <inheritdoc/>
    public override string ToString()
        => Value.ToString("yyyy-MM-dd");
}