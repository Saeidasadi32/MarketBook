// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactionById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactionById;

/// <summary>
/// EN: Represents one persisted portfolio transaction.
/// FA: یک تراکنش ماندگار پرتفوی را نمایش می‌دهد.
/// </summary>
/// <param name="Id">EN: Transaction identifier. FA: شناسه تراکنش.</param>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="ListingId">EN: Listing identifier. FA: شناسه Listing.</param>
/// <param name="CurrencyId">EN: Quote-currency identifier at execution time. FA: شناسه ارز مظنه در زمان اجرا.</param>
/// <param name="Type">EN: Transaction type. FA: نوع تراکنش.</param>
/// <param name="Quantity">EN: Executed quantity. FA: تعداد اجراشده.</param>
/// <param name="Price">EN: Executed price. FA: قیمت اجراشده.</param>
/// <param name="Commission">EN: Commission. FA: کمیسیون.</param>
/// <param name="Tax">EN: Tax. FA: مالیات.</param>
/// <param name="ExchangeFee">EN: Exchange fee. FA: کارمزد بورس.</param>
/// <param name="BrokerFee">EN: Broker fee. FA: کارمزد کارگزار.</param>
/// <param name="ClearingFee">EN: Clearing fee. FA: کارمزد پایاپای.</param>
/// <param name="OtherFees">EN: Other fees. FA: سایر هزینه‌ها.</param>
/// <param name="GrossValue">EN: Gross transaction value. FA: ارزش ناخالص تراکنش.</param>
/// <param name="TotalCosts">EN: Total transaction costs. FA: مجموع هزینه‌های تراکنش.</param>
/// <param name="ExecutedOn">EN: Execution timestamp. FA: زمان اجرای تراکنش.</param>
/// <param name="CreatedOn">EN: Persistence creation timestamp. FA: زمان ایجاد رکورد.</param>
public sealed record GetPortfolioTransactionByIdResponse(
    string Id,
    string PortfolioId,
    string ListingId,
    string CurrencyId,
    int Type,
    decimal Quantity,
    decimal Price,
    decimal Commission,
    decimal Tax,
    decimal ExchangeFee,
    decimal BrokerFee,
    decimal ClearingFee,
    decimal OtherFees,
    decimal GrossValue,
    decimal TotalCosts,
    DateTimeOffset ExecutedOn,
    DateTimeOffset CreatedOn);
