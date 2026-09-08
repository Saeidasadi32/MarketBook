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
/// EN: Defines supported cash-ledger transaction types and their balance direction.
/// FA: انواع پشتیبانی‌شده تراکنش دفتر نقدی و جهت اثر آن‌ها بر موجودی را تعریف می‌کند.
/// </summary>
public enum PortfolioCashTransactionType
{
    /// <summary>EN: External cash deposit. FA: واریز وجه از خارج پرتفوی.</summary>
    Deposit = 1,
    /// <summary>EN: External cash withdrawal. FA: برداشت وجه از پرتفوی.</summary>
    Withdrawal = 2,
    /// <summary>EN: Cash paid to settle a buy. FA: وجه پرداختی بابت تسویه خرید.</summary>
    BuySettlement = 3,
    /// <summary>EN: Cash received from a sell. FA: وجه دریافتی بابت تسویه فروش.</summary>
    SellSettlement = 4,
    /// <summary>EN: Cash fee. FA: کارمزد نقدی.</summary>
    Fee = 5,
    /// <summary>EN: Cash tax. FA: مالیات نقدی.</summary>
    Tax = 6,
    /// <summary>EN: Dividend receipt. FA: دریافت سود نقدی.</summary>
    Dividend = 7,
    /// <summary>EN: Interest receipt. FA: دریافت بهره/سود مالی.</summary>
    Interest = 8,
    /// <summary>EN: Other cash credit. FA: سایر ورودی‌های نقدی.</summary>
    OtherCredit = 9,
    /// <summary>EN: Other cash debit. FA: سایر خروجی‌های نقدی.</summary>
    OtherDebit = 10
}
