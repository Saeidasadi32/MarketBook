using MediatR;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Portfolios.Queries.EvaluatePortfolioRiskLimits;
using MarketBook.Application.Features.Portfolios.RiskEvaluations.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskEvaluation.Aggregates;
using MarketBook.Domain.PortfolioRiskEvaluation.Entities;
using MarketBook.Domain.PortfolioRiskEvaluation.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.ValueObjects;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Portfolios.RiskEvaluations.Commands.CreatePortfolioRiskEvaluation;

/// <summary>Handles calculation and persistence of portfolio risk-evaluation snapshots.</summary>
public sealed class CreatePortfolioRiskEvaluationCommandHandler
    : IRequestHandler<CreatePortfolioRiskEvaluationCommand, Result<PortfolioRiskEvaluationResponse>>
{
    private readonly ISender _sender;
    private readonly IPortfolioRiskEvaluationRepository _evaluations;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>Initializes a new create-risk-evaluation command handler.</summary>
    public CreatePortfolioRiskEvaluationCommandHandler(
        ISender sender,
        IPortfolioRiskEvaluationRepository evaluations,
        IApplicationDbContext dbContext)
    {
        _sender = sender;
        _evaluations = evaluations;
        _dbContext = dbContext;
    }

    /// <summary>Calculates and persists an official portfolio risk-evaluation snapshot.</summary>
    public async Task<Result<PortfolioRiskEvaluationResponse>> Handle(
        CreatePortfolioRiskEvaluationCommand request,
        CancellationToken cancellationToken)
    {
        Result<EvaluatePortfolioRiskLimitsResponse> evaluationResult =
            await _sender.Send(
                new EvaluatePortfolioRiskLimitsQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval,
                    request.ConfidenceLevel,
                    request.RiskFreeRateAnnual,
                    request.MinimumAcceptableReturnAnnual,
                    request.MaxAnnualizedVolatility,
                    request.MaxValueAtRiskReturn,
                    request.MaxValueAtRiskAmountBase,
                    request.MaxDrawdownLossRatio,
                    request.MaxDrawdownAmountBase,
                    request.MinSharpeRatio,
                    request.MinSortinoRatio),
                cancellationToken);

        if (evaluationResult.IsFailure)
        {
            return Result<PortfolioRiskEvaluationResponse>.Fail(
                evaluationResult.Error);
        }

        EvaluatePortfolioRiskLimitsResponse source = evaluationResult.Value!;

        PortfolioId portfolioId = PortfolioId.Parse(source.PortfolioId);

        PortfolioRiskPolicyId? policyId =
            string.IsNullOrWhiteSpace(source.PolicyId)
                ? null
                : PortfolioRiskPolicyId.Parse(source.PolicyId);

        PortfolioRiskEvaluation snapshot = new(
            PortfolioRiskEvaluationId.New(),
            portfolioId,
            source.BaseCurrencyId,
            source.From,
            source.To,
            source.Interval,
            request.ConfidenceLevel,
            request.RiskFreeRateAnnual,
            request.MinimumAcceptableReturnAnnual,
            source.PolicyAsOf,
            source.LimitSource,
            policyId,
            source.PolicyVersion,
            source.IsComplete,
            source.OverallStatus,
            source.ConfiguredLimitCount,
            source.BreachedLimitCount,
            source.NotCalculableLimitCount,
            source.Rules.Select(rule =>
                new PortfolioRiskEvaluationRule(
                    rule.Code,
                    rule.Direction,
                    rule.IsConfigured,
                    rule.Status,
                    rule.Limit,
                    rule.Actual,
                    rule.BreachAmount)));

        await _evaluations.AddAsync(snapshot, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<PortfolioRiskEvaluationResponse>.Success(snapshot.ToResponse());
    }
}
