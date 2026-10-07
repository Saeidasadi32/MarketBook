using MarketBook.Domain.PortfolioRiskEvaluation.Aggregates;
using MarketBook.Domain.PortfolioRiskEvaluation.Entities;

namespace MarketBook.Application.Features.Portfolios.RiskEvaluations.Common;

internal static class PortfolioRiskEvaluationMappings
{
    public static PortfolioRiskEvaluationResponse ToResponse(
        this PortfolioRiskEvaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(evaluation);

        return new PortfolioRiskEvaluationResponse(
            evaluation.Id.Value.ToString(),
            evaluation.PortfolioId.Value.ToString(),
            evaluation.BaseCurrencyId,
            evaluation.From,
            evaluation.To,
            evaluation.Interval,
            evaluation.ConfidenceLevel,
            evaluation.RiskFreeRateAnnual,
            evaluation.MinimumAcceptableReturnAnnual,
            evaluation.PolicyAsOf,
            evaluation.LimitSource,
            evaluation.PolicyId?.Value.ToString(),
            evaluation.PolicyVersion,
            evaluation.IsComplete,
            evaluation.OverallStatus,
            evaluation.ConfiguredLimitCount,
            evaluation.BreachedLimitCount,
            evaluation.NotCalculableLimitCount,
            evaluation.EvaluatedOn,
            evaluation.Rules.Select(rule =>
                new PortfolioRiskEvaluationRuleResponse(
                    rule.Code,
                    rule.Direction,
                    rule.IsConfigured,
                    rule.Status,
                    rule.Limit,
                    rule.Actual,
                    rule.BreachAmount))
                .ToArray());
    }
}
