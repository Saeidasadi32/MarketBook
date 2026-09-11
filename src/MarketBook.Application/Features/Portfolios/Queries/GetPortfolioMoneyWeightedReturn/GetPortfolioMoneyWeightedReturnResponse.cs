// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMoneyWeightedReturn
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMoneyWeightedReturn;

/// <summary>
/// EN: One dated XIRR cash flow expressed in portfolio base currency.
/// FA: یک جریان نقدی تاریخ‌دار XIRR در ارز پایه پرتفوی.
/// </summary>
/// <param name="Date">EN: Cash-flow instant. FA: لحظه جریان نقدی.</param>
/// <param name="AmountBase">EN: Investor-perspective signed cash flow in base currency. Contributions are negative; withdrawals and terminal value are positive. FA: جریان نقدی علامت‌دار از دید سرمایه‌گذار در ارز پایه؛ آورده منفی و برداشت و ارزش نهایی مثبت است.</param>
/// <param name="Kind">EN: BeginningNAV, Deposit, Withdrawal, or TerminalNAV. FA: نوع جریان: NAV ابتدا، واریز، برداشت یا NAV نهایی.</param>
public sealed record PortfolioXirrCashFlowResponse(
    DateTimeOffset Date,
    decimal AmountBase,
    string Kind);

/// <summary>
/// EN: Historical portfolio money-weighted return response.
/// FA: پاسخ بازده پول‌وزن تاریخی پرتفوی.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency identifier. FA: شناسه ارز پایه پرتفوی.</param>
/// <param name="From">EN: Beginning valuation instant. FA: لحظه ارزش‌گذاری ابتدای دوره.</param>
/// <param name="To">EN: Ending valuation instant. FA: لحظه ارزش‌گذاری انتهای دوره.</param>
/// <param name="IsComplete">EN: True when beginning NAV, ending NAV, and all external-flow FX translations are complete. FA: وقتی NAV ابتدا و انتها و ترجمه FX همه جریان‌های خارجی کامل باشد true است.</param>
/// <param name="HasValidCashFlowSigns">EN: True when the XIRR stream contains at least one negative and one positive amount. FA: وقتی جریان XIRR حداقل یک مبلغ منفی و یک مبلغ مثبت داشته باشد true است.</param>
/// <param name="HasSolution">EN: True when a stable XIRR root is found within the supported search domain. FA: وقتی یک ریشه پایدار XIRR در دامنه جست‌وجوی پشتیبانی‌شده پیدا شود true است.</param>
/// <param name="AnnualizedMoneyWeightedReturn">EN: Annualized XIRR decimal fraction, or null when unavailable. FA: بازده پول‌وزن سالانه‌شده به‌صورت کسر اعشاری، یا null در صورت عدم دسترسی.</param>
/// <param name="CashFlows">EN: Dated cash flows used by the solver. FA: جریان‌های نقدی تاریخ‌دار استفاده‌شده توسط solver.</param>
public sealed record GetPortfolioMoneyWeightedReturnResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    bool IsComplete,
    bool HasValidCashFlowSigns,
    bool HasSolution,
    decimal? AnnualizedMoneyWeightedReturn,
    IReadOnlyCollection<PortfolioXirrCashFlowResponse> CashFlows);
