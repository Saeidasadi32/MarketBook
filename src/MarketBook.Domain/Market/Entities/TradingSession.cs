// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Calendar.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Calendar.Entities;

/// <summary>
/// EN: Represents the regular trading hours for one day of the week.
/// FA: ساعات معاملاتی عادی یک روز هفته را نمایش می‌دهد.
/// </summary>
public sealed class TradingSession
{
    /// <summary>
    /// EN: Initializes a trading session.
    /// FA: یک Session معاملاتی را ایجاد می‌کند.
    /// </summary>
    public TradingSession(
        DayOfWeek dayOfWeek,
        TimeOnly opensAt,
        TimeOnly closesAt)
    {
        if (closesAt <= opensAt)
        {
            throw new ArgumentException(
                "Trading session close time must be after open time.",
                nameof(closesAt));
        }

        DayOfWeek = dayOfWeek;
        OpensAt = opensAt;
        ClosesAt = closesAt;
    }

    private TradingSession()
    {
    }

    /// <summary>
    /// EN: Gets the week day.
    /// FA: روز هفته را دریافت می‌کند.
    /// </summary>
    public DayOfWeek DayOfWeek { get; private set; }

    /// <summary>
    /// EN: Gets the regular opening time in market-local time.
    /// FA: زمان عادی بازگشایی را به وقت محلی بازار دریافت می‌کند.
    /// </summary>
    public TimeOnly OpensAt { get; private set; }

    /// <summary>
    /// EN: Gets the regular closing time in market-local time.
    /// FA: زمان عادی پایان معاملات را به وقت محلی بازار دریافت می‌کند.
    /// </summary>
    public TimeOnly ClosesAt { get; private set; }
}
