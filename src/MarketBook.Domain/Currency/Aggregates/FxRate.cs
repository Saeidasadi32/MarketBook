// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Currency.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;

namespace MarketBook.Domain.Currency.Aggregates;

/// <summary>
/// EN: Represents a dated FX quote where one unit of BaseCurrency equals Rate units of QuoteCurrency.
/// FA: نرخ تاریخ‌دار ارز را نمایش می‌دهد که در آن یک واحد ارز پایه برابر Rate واحد ارز مظنه است.
/// </summary>
public sealed class FxRate : AggregateRoot<FxRateId>
{
    /// <summary>
    /// EN: Initializes a new FX-rate record.
    /// FA: یک رکورد جدید نرخ تبدیل ارز را ایجاد می‌کند.
    /// </summary>
    /// <param name="id">EN: FX-rate identifier. FA: شناسه نرخ ارز.</param>
    /// <param name="baseCurrencyId">EN: Base currency identifier. FA: شناسه ارز پایه.</param>
    /// <param name="quoteCurrencyId">EN: Quote currency identifier. FA: شناسه ارز مظنه.</param>
    /// <param name="rateDate">EN: Effective rate date. FA: تاریخ موثر نرخ.</param>
    /// <param name="rate">EN: Quote units per one base unit. FA: تعداد واحد ارز مظنه به ازای یک واحد ارز پایه.</param>
    public FxRate(
        FxRateId id,
        CurrencyId baseCurrencyId,
        CurrencyId quoteCurrencyId,
        DateOnly rateDate,
        decimal rate)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(baseCurrencyId);
        ArgumentNullException.ThrowIfNull(quoteCurrencyId);

        if (baseCurrencyId == quoteCurrencyId)
        {
            throw new ArgumentException(
                "Base and quote currencies must be different.",
                nameof(quoteCurrencyId));
        }

        if (rate <= 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rate),
                "FX rate must be greater than zero.");
        }

        BaseCurrencyId = baseCurrencyId;
        QuoteCurrencyId = quoteCurrencyId;
        RateDate = rateDate;
        Rate = rate;
        CreatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private FxRate()
    {
        BaseCurrencyId = default!;
        QuoteCurrencyId = default!;
    }

    /// <summary>
    /// EN: Gets the base currency identifier.
    /// FA: شناسه ارز پایه را دریافت می‌کند.
    /// </summary>
    public CurrencyId BaseCurrencyId { get; private set; }

    /// <summary>
    /// EN: Gets the quote currency identifier.
    /// FA: شناسه ارز مظنه را دریافت می‌کند.
    /// </summary>
    public CurrencyId QuoteCurrencyId { get; private set; }

    /// <summary>
    /// EN: Gets the effective rate date.
    /// FA: تاریخ موثر نرخ را دریافت می‌کند.
    /// </summary>
    public DateOnly RateDate { get; private set; }

    /// <summary>
    /// EN: Gets quote-currency units per one base-currency unit.
    /// FA: تعداد واحد ارز مظنه به ازای یک واحد ارز پایه را دریافت می‌کند.
    /// </summary>
    public decimal Rate { get; private set; }

    /// <summary>
    /// EN: Gets the creation timestamp.
    /// FA: زمان ایجاد رکورد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// EN: Creates a new FX-rate aggregate.
    /// FA: یک Aggregate جدید نرخ ارز ایجاد می‌کند.
    /// </summary>
    /// <param name="baseCurrencyId">EN: Base currency identifier. FA: شناسه ارز پایه.</param>
    /// <param name="quoteCurrencyId">EN: Quote currency identifier. FA: شناسه ارز مظنه.</param>
    /// <param name="rateDate">EN: Effective rate date. FA: تاریخ موثر نرخ.</param>
    /// <param name="rate">EN: Quote units per one base unit. FA: تعداد واحد ارز مظنه به ازای یک واحد ارز پایه.</param>
    /// <returns>EN: New FX-rate aggregate. FA: Aggregate جدید نرخ ارز.</returns>
    public static FxRate Create(
        CurrencyId baseCurrencyId,
        CurrencyId quoteCurrencyId,
        DateOnly rateDate,
        decimal rate)
        => new(
            FxRateId.New(),
            baseCurrencyId,
            quoteCurrencyId,
            rateDate,
            rate);
}
