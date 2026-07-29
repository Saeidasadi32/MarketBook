// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Financial.ValueObjects;

namespace MarketBook.Domain.Portfolio.ValueObjects;

/// <summary>
/// EN: Represents all costs associated with a transaction.
/// FA: تمام هزینه‌های مربوط به یک معامله را نمایش می‌دهد.
/// </summary>
public sealed record TransactionCost
{
    public TransactionCost(
        Money commission,
        Money tax,
        Money exchangeFee,
        Money brokerFee,
        Money clearingFee,
        Money otherFees)
    {
        Commission = commission;
        Tax = tax;
        ExchangeFee = exchangeFee;
        BrokerFee = brokerFee;
        ClearingFee = clearingFee;
        OtherFees = otherFees;
    }

    /// <summary>
    /// EN: Broker commission.
    /// FA: کارمزد کارگزاری.
    /// </summary>
    public Money Commission { get; }

    /// <summary>
    /// EN: Tax.
    /// FA: مالیات.
    /// </summary>
    public Money Tax { get; }

    /// <summary>
    /// EN: Exchange fee.
    /// FA: کارمزد بورس.
    /// </summary>
    public Money ExchangeFee { get; }

    /// <summary>
    /// EN: Broker fee.
    /// FA: سایر کارمزدهای کارگزار.
    /// </summary>
    public Money BrokerFee { get; }

    /// <summary>
    /// EN: Clearing fee.
    /// FA: کارمزد اتاق پایاپای.
    /// </summary>
    public Money ClearingFee { get; }

    /// <summary>
    /// EN: Other costs.
    /// FA: سایر هزینه‌ها.
    /// </summary>
    public Money OtherFees { get; }

    /// <summary>
    /// EN: Total transaction costs.
    /// FA: مجموع هزینه‌های معامله.
    /// </summary>
    public Money Total =>
        Commission +
        Tax +
        ExchangeFee +
        BrokerFee +
        ClearingFee +
        OtherFees;

    /// <summary>
    /// EN: Represents zero costs.
    /// FA: بدون هزینه.
    /// </summary>
    public static TransactionCost Zero =>
        new(
            Money.Zero,
            Money.Zero,
            Money.Zero,
            Money.Zero,
            Money.Zero,
            Money.Zero);
}