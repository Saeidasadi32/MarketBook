// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;

namespace MarketBook.Application.Features.PortfolioRiskPolicies;

/// <summary>
/// EN: Request body containing persisted hard risk limits.
/// FA: بدنه درخواست شامل حدود سخت ریسک ذخیره‌شونده.
/// </summary>
public sealed record PortfolioRiskPolicyLimitsRequest(
    DateTimeOffset EffectiveFrom,
    decimal? MaxAnnualizedVolatility,
    decimal? MaxValueAtRiskReturn,
    decimal? MaxValueAtRiskAmountBase,
    decimal? MaxDrawdownLossRatio,
    decimal? MaxDrawdownAmountBase,
    decimal? MinSharpeRatio,
    decimal? MinSortinoRatio);

/// <summary>
/// EN: API representation of a persisted portfolio risk-policy version.
/// FA: نمایش API یک نسخه ذخیره‌شده سیاست ریسک پرتفوی.
/// </summary>
public sealed record PortfolioRiskPolicyResponse(
    string Id,
    string PortfolioId,
    int PolicyVersion,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset? EffectiveTo,
    string Status,
    decimal? MaxAnnualizedVolatility,
    decimal? MaxValueAtRiskReturn,
    decimal? MaxValueAtRiskAmountBase,
    decimal? MaxDrawdownLossRatio,
    decimal? MaxDrawdownAmountBase,
    decimal? MinSharpeRatio,
    decimal? MinSortinoRatio,
    DateTimeOffset CreatedOn,
    DateTimeOffset UpdatedOn)
{
    /// <summary>EN: Maps a domain policy. FA: Domain Policy را Map می‌کند.</summary>
    public static PortfolioRiskPolicyResponse FromDomain(PortfolioRiskPolicy policy)
        => new(
            policy.Id.Value.ToString(),
            policy.PortfolioId.Value.ToString(),
            policy.PolicyVersion,
            policy.EffectiveFrom,
            policy.EffectiveTo,
            policy.Status.ToString(),
            policy.MaxAnnualizedVolatility,
            policy.MaxValueAtRiskReturn,
            policy.MaxValueAtRiskAmountBase,
            policy.MaxDrawdownLossRatio,
            policy.MaxDrawdownAmountBase,
            policy.MinSharpeRatio,
            policy.MinSortinoRatio,
            policy.CreatedOn,
            policy.UpdatedOn);
}
