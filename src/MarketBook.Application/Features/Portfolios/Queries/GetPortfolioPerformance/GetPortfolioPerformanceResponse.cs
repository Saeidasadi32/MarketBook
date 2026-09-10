// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformance
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformance;

/// <summary>
/// EN: One external portfolio cash flow translated into base currency at its own occurrence date.
/// FA: یک جریان نقدی خارجی پرتفوی که در تاریخ وقوع خودش به ارز پایه ترجمه شده است.
/// </summary>
/// <param name="CashTransactionId">EN: Cash-ledger transaction identifier. FA: شناسه تراکنش دفتر نقدی.</param>
/// <param name="CurrencyId">EN: Source currency identifier. FA: شناسه ارز مبدا.</param>
/// <param name="Type">EN: External-flow type. FA: نوع جریان خارجی.</param>
/// <param name="OccurredOn">EN: Business occurrence instant. FA: لحظه وقوع تجاری.</param>
/// <param name="SignedSourceAmount">EN: Signed source amount; deposits are positive and withdrawals are negative. FA: مبلغ علامت‌دار مبدا؛ واریز مثبت و برداشت منفی است.</param>
/// <param name="IsFxAvailable">EN: True when an eligible historical FX conversion exists. FA: وقتی نرخ FX تاریخی معتبر موجود باشد true است.</param>
/// <param name="FxRateDate">EN: FX rate date used for this flow. FA: تاریخ نرخ FX استفاده‌شده برای این جریان.</param>
/// <param name="SourceToBaseRate">EN: Effective source-to-base conversion rate. FA: نرخ موثر تبدیل ارز مبدا به ارز پایه.</param>
/// <param name="SignedAmountBase">EN: Signed external flow translated to base currency, or null when FX is unavailable. FA: جریان خارجی علامت‌دار در ارز پایه، یا null در صورت نبود FX.</param>
public sealed record PortfolioExternalCashFlowResponse(
    string CashTransactionId,
    string CurrencyId,
    string Type,
    DateTimeOffset OccurredOn,
    decimal SignedSourceAmount,
    bool IsFxAvailable,
    DateOnly? FxRateDate,
    decimal? SourceToBaseRate,
    decimal? SignedAmountBase);

/// <summary>
/// EN: Cash-flow-aware historical performance foundation in portfolio base currency.
/// FA: مبنای عملکرد تاریخی پرتفوی با لحاظ جریان نقدی خارجی در ارز پایه پرتفوی.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency identifier. FA: شناسه ارز پایه پرتفوی.</param>
/// <param name="From">EN: Beginning NAV cutoff. FA: لحظه NAV ابتدای دوره.</param>
/// <param name="To">EN: Ending NAV cutoff. FA: لحظه NAV انتهای دوره.</param>
/// <param name="IsBeginningNavComplete">EN: True when beginning NAV is fully priced and translated. FA: وقتی NAV ابتدای دوره کاملاً قیمت‌گذاری و ترجمه شده باشد true است.</param>
/// <param name="IsEndingNavComplete">EN: True when ending NAV is fully priced and translated. FA: وقتی NAV انتهای دوره کاملاً قیمت‌گذاری و ترجمه شده باشد true است.</param>
/// <param name="AreExternalFlowsFullyTranslated">EN: True when every external flow can be translated at its occurrence date. FA: وقتی همه جریان‌های خارجی در تاریخ وقوع قابل ترجمه باشند true است.</param>
/// <param name="IsComplete">EN: True when both boundary NAVs and all external flows are complete. FA: وقتی هر دو NAV مرزی و همه جریان‌های خارجی کامل باشند true است.</param>
/// <param name="BeginningNetAssetValueBase">EN: Complete beginning NAV in base currency. FA: NAV کامل ابتدای دوره در ارز پایه.</param>
/// <param name="EndingNetAssetValueBase">EN: Complete ending NAV in base currency. FA: NAV کامل انتهای دوره در ارز پایه.</param>
/// <param name="NetExternalFlowBase">EN: Net external flow over (From, To] in base currency. FA: خالص جریان خارجی در بازه (From, To] در ارز پایه.</param>
/// <param name="InvestmentProfitLossBase">EN: Investment result before defining a return methodology: Ending NAV - Beginning NAV - Net External Flow. FA: نتیجه سرمایه‌گذاری پیش از تعریف روش بازده: NAV پایان منهای NAV ابتدا منهای خالص جریان خارجی.</param>
/// <param name="ExternalFlows">EN: External cash flows in deterministic order. FA: جریان‌های نقدی خارجی با ترتیب قطعی.</param>
public sealed record GetPortfolioPerformanceResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    bool IsBeginningNavComplete,
    bool IsEndingNavComplete,
    bool AreExternalFlowsFullyTranslated,
    bool IsComplete,
    decimal? BeginningNetAssetValueBase,
    decimal? EndingNetAssetValueBase,
    decimal? NetExternalFlowBase,
    decimal? InvestmentProfitLossBase,
    IReadOnlyCollection<PortfolioExternalCashFlowResponse> ExternalFlows);
