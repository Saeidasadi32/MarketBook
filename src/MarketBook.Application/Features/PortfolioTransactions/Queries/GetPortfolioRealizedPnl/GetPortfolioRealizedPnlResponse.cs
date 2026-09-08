// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioRealizedPnl
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioRealizedPnl;

/// <summary>
/// EN: Realized P/L projection for one listing in its transaction currency.
/// FA: تصویر سود/زیان تحقق‌یافته یک لیستینگ در ارز تراکنش‌های آن.
/// </summary>
/// <param name="ListingId">EN: Listing identifier. FA: شناسه لیستینگ.</param>
/// <param name="CurrencyId">EN: Transaction currency identifier. FA: شناسه ارز تراکنش.</param>
/// <param name="SoldQuantity">EN: Total realized/sold quantity. FA: مجموع مقدار فروخته‌شده/تحقق‌یافته.</param>
/// <param name="GrossProceeds">EN: Gross proceeds before selling costs. FA: عایدی ناخالص پیش از هزینه‌های فروش.</param>
/// <param name="SellingCosts">EN: Total costs attached to sell transactions. FA: مجموع هزینه‌های مربوط به معاملات فروش.</param>
/// <param name="NetProceeds">EN: Gross proceeds less selling costs. FA: عایدی خالص پس از کسر هزینه‌های فروش.</param>
/// <param name="CostBasis">EN: Acquisition cost relieved by sold quantity. FA: بهای تمام‌شده تخصیص‌یافته به مقدار فروخته‌شده.</param>
/// <param name="RealizedProfitLoss">EN: Net proceeds less relieved cost basis. FA: عایدی خالص منهای بهای تمام‌شده تخصیص‌یافته.</param>
/// <param name="RemainingQuantity">EN: Open quantity remaining after all ledger entries. FA: مقدار باز باقی‌مانده پس از تمام رکوردهای دفتر.</param>
/// <param name="RemainingBookCost">EN: Remaining acquisition book cost. FA: بهای دفتری خریدِ باقی‌مانده.</param>
/// <param name="RemainingAverageCost">EN: Remaining weighted-average acquisition cost. FA: میانگین موزون بهای خریدِ باقی‌مانده.</param>
public sealed record PortfolioRealizedPnlItemResponse(
    string ListingId,
    string CurrencyId,
    decimal SoldQuantity,
    decimal GrossProceeds,
    decimal SellingCosts,
    decimal NetProceeds,
    decimal CostBasis,
    decimal RealizedProfitLoss,
    decimal RemainingQuantity,
    decimal RemainingBookCost,
    decimal RemainingAverageCost);

/// <summary>
/// EN: Realized P/L projection response. Values are not aggregated across currencies.
/// FA: پاسخ تصویر سود/زیان تحقق‌یافته. مقادیر بین ارزهای مختلف با یکدیگر جمع نمی‌شوند.
/// </summary>
/// <param name="CostBasisMethod">EN: Applied cost-basis method code. FA: کد روش بهای تمام‌شده اعمال‌شده.</param>
/// <param name="Items">EN: Per-listing realized P/L items. FA: اقلام سود/زیان تحقق‌یافته به تفکیک لیستینگ.</param>
public sealed record GetPortfolioRealizedPnlResponse(
    int CostBasisMethod,
    IReadOnlyCollection<PortfolioRealizedPnlItemResponse> Items);
