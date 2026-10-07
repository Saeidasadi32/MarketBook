using MediatR;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Portfolios.RiskEvaluations.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskEvaluation.Aggregates;
using MarketBook.Domain.PortfolioRiskEvaluation.ValueObjects;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Portfolios.RiskEvaluations.Queries.GetPortfolioRiskEvaluationById;

/// <summary>Handles retrieval of a persisted portfolio risk-evaluation snapshot.</summary>
public sealed class GetPortfolioRiskEvaluationByIdQueryHandler
    : IRequestHandler<GetPortfolioRiskEvaluationByIdQuery, Result<PortfolioRiskEvaluationResponse>>
{
    private readonly IPortfolioRiskEvaluationRepository _evaluations;

    /// <summary>Initializes a new risk-evaluation lookup query handler.</summary>
    public GetPortfolioRiskEvaluationByIdQueryHandler(
        IPortfolioRiskEvaluationRepository evaluations)
    {
        _evaluations = evaluations;
    }

    /// <summary>Gets a persisted risk-evaluation snapshot by identifier.</summary>
    public async Task<Result<PortfolioRiskEvaluationResponse>> Handle(
        GetPortfolioRiskEvaluationByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<PortfolioRiskEvaluationResponse>.Fail(
                new Error(
                    "PortfolioRiskEvaluation.PortfolioId.Invalid",
                    "The portfolio identifier is invalid."));
        }

        if (!PortfolioRiskEvaluationId.TryParse(
                request.EvaluationId,
                out PortfolioRiskEvaluationId? evaluationId) ||
            evaluationId is null)
        {
            return Result<PortfolioRiskEvaluationResponse>.Fail(
                new Error(
                    "PortfolioRiskEvaluation.Id.Invalid",
                    "The risk-evaluation identifier is invalid."));
        }

        PortfolioRiskEvaluation? evaluation =
            await _evaluations.GetByIdAsync(evaluationId, cancellationToken);

        if (evaluation is null ||
            evaluation.PortfolioId != portfolioId)
        {
            return Result<PortfolioRiskEvaluationResponse>.Fail(
                new Error(
                    "PortfolioRiskEvaluation.NotFound",
                    "The portfolio risk evaluation was not found."));
        }

        return Result<PortfolioRiskEvaluationResponse>.Success(evaluation.ToResponse());
    }
}
