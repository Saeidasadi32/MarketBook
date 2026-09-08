// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactions;

/// <summary>
/// EN: Represents one transaction item in a paged ledger.
/// FA: یک آیتم تراکنش در دفتر صفحه‌بندی‌شده را نمایش می‌دهد.
/// </summary>
/// <param name="Id">EN: Transaction identifier. FA: شناسه تراکنش.</param>
/// <param name="ListingId">EN: Listing identifier. FA: شناسه Listing.</param>
/// <param name="CurrencyId">EN: Quote-currency identifier. FA: شناسه ارز مظنه.</param>
/// <param name="Type">EN: Transaction type. FA: نوع تراکنش.</param>
/// <param name="Quantity">EN: Executed quantity. FA: تعداد اجراشده.</param>
/// <param name="Price">EN: Executed price. FA: قیمت اجراشده.</param>
/// <param name="TotalCosts">EN: Total transaction costs. FA: مجموع هزینه‌های تراکنش.</param>
/// <param name="ExecutedOn">EN: Execution timestamp. FA: زمان اجرا.</param>
public sealed record PortfolioTransactionItemResponse(
    string Id,
    string ListingId,
    string CurrencyId,
    int Type,
    decimal Quantity,
    decimal Price,
    decimal TotalCosts,
    DateTimeOffset ExecutedOn);

/// <summary>
/// EN: Represents a paged portfolio transaction result.
/// FA: نتیجه صفحه‌بندی‌شده تراکنش‌های پرتفوی را نمایش می‌دهد.
/// </summary>
/// <param name="Items">EN: Result items. FA: آیتم‌های نتیجه.</param>
/// <param name="Page">EN: Normalized page. FA: صفحه نرمال‌شده.</param>
/// <param name="PageSize">EN: Normalized page size. FA: اندازه صفحه نرمال‌شده.</param>
/// <param name="TotalCount">EN: Total matching count. FA: تعداد کل نتایج.</param>
public sealed record GetPortfolioTransactionsResponse(
    IReadOnlyCollection<PortfolioTransactionItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
