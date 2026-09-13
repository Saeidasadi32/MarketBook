// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.EvaluatePortfolioRiskLimits
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.EvaluatePortfolioRiskLimits;

/// <summary>
/// EN: One auditable risk-limit evaluation.
/// FA: یک ارزیابی قابل ممیزی از حد ریسک.
/// </summary>
/// <param name="Code">EN: Stable rule code. FA: کد پایدار Rule.</param>
/// <param name="Direction">EN: Maximum or Minimum threshold direction. FA: جهت آستانه Maximum یا Minimum.</param>
/// <param name="IsConfigured">EN: True when this limit was supplied. FA: وقتی این Limit ارسال شده باشد true است.</param>
/// <param name="Status">EN: NotConfigured, NotCalculable, WithinLimit, or Breached. FA: وضعیت ارزیابی Rule.</param>
/// <param name="Limit">EN: Configured threshold. FA: آستانه پیکربندی‌شده.</param>
/// <param name="Actual">EN: Actual source metric. FA: مقدار واقعی Metric منبع.</param>
/// <param name="BreachAmount">EN: Positive threshold exceedance when breached; otherwise null. FA: میزان مثبت عبور از حد در حالت Breach؛ در غیر این صورت null.</param>
public sealed record PortfolioRiskLimitEvaluationResponse(
    string Code,
    string Direction,
    bool IsConfigured,
    string Status,
    decimal? Limit,
    decimal? Actual,
    decimal? BreachAmount);

/// <summary>
/// EN: Portfolio risk-limit evaluation result resolved from request overrides and/or the active persisted policy.
/// FA: نتیجه ارزیابی حدود ریسک پرتفوی که از Overrideهای Request و/یا Policy فعال ذخیره‌شده resolve شده است.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="BaseCurrencyId">EN: Portfolio base currency. FA: ارز پایه پرتفوی.</param>
/// <param name="From">EN: Beginning instant. FA: لحظه شروع.</param>
/// <param name="To">EN: Ending instant. FA: لحظه پایان.</param>
/// <param name="Interval">EN: Resolved interval. FA: فاصله resolve‌شده.</param>
/// <param name="LimitSource">EN: None, PersistedPolicy, RequestOverride, or Mixed. FA: منبع Limitهای resolve‌شده.</param>
/// <param name="PolicyId">EN: Active persisted policy identifier when one participated in resolution. FA: شناسه Policy فعال ذخیره‌شده در صورت مشارکت در resolve.</param>
/// <param name="PolicyVersion">EN: Active persisted business version when one participated in resolution. FA: نسخه کسب‌وکاری Policy فعال ذخیره‌شده در صورت مشارکت در resolve.</param>
/// <param name="IsComplete">EN: Mirrors DOC-0043 source completeness. FA: کامل‌بودن منبع DOC-0043 را منعکس می‌کند.</param>
/// <param name="OverallStatus">EN: Incomplete, NoLimitsConfigured, Indeterminate, WithinLimit, or Breached. FA: وضعیت کلی ارزیابی.</param>
/// <param name="ConfiguredLimitCount">EN: Number of configured limits. FA: تعداد Limitهای پیکربندی‌شده.</param>
/// <param name="BreachedLimitCount">EN: Number of breached limits. FA: تعداد Limitهای نقض‌شده.</param>
/// <param name="NotCalculableLimitCount">EN: Number of configured limits without calculable metrics. FA: تعداد Limitهای پیکربندی‌شده با Metric غیرقابل محاسبه.</param>
/// <param name="Rules">EN: Ordered risk-limit evaluations. FA: ارزیابی‌های مرتب حدود ریسک.</param>
public sealed record EvaluatePortfolioRiskLimitsResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Interval,
    string LimitSource,
    string? PolicyId,
    int? PolicyVersion,
    bool IsComplete,
    string OverallStatus,
    int ConfiguredLimitCount,
    int BreachedLimitCount,
    int NotCalculableLimitCount,
    IReadOnlyCollection<PortfolioRiskLimitEvaluationResponse> Rules);
