using MediatR;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Portfolios.RiskEvaluations.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskEvaluation.Aggregates;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Portfolios.RiskEvaluations.Queries.GetPortfolioRiskEvaluationHistory;

/// <summary>Handles retrieval of persisted portfolio risk-evaluation history.</summary>
public sealed class GetPortfolioRiskEvaluationHistoryQueryHandler
    : IRequestHandler<GetPortfolioRiskEvaluationHistoryQuery, Result<IReadOnlyCollection<PortfolioRiskEvaluationResponse>>>
{
    private readonly IPortfolioRiskEvaluationRepository _evaluations;

    /// <summary>Initializes a new risk-evaluation history query handler.</summary>
    public GetPortfolioRiskEvaluationHistoryQueryHandler(
        IPortfolioRiskEvaluationRepository evaluations)
    {
        _evaluations = evaluations;
    }

    /// <summary>Gets persisted risk-evaluation history for a portfolio.</summary>
    public async Task<Result<IReadOnlyCollection<PortfolioRiskEvaluationResponse>>> Handle(
        GetPortfolioRiskEvaluationHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<IReadOnlyCollection<PortfolioRiskEvaluationResponse>>.Fail(
                new Error(
                    "PortfolioRiskEvaluation.PortfolioId.Invalid",
                    "The portfolio identifier is invalid."));
        }

        IReadOnlyCollection<PortfolioRiskEvaluation> evaluations =
            await _evaluations.GetHistoryAsync(portfolioId, cancellationToken);

        IReadOnlyCollection<PortfolioRiskEvaluationResponse> response =
            evaluations.Select(item => item.ToResponse()).ToArray();

        return Result<IReadOnlyCollection<PortfolioRiskEvaluationResponse>>.Success(response);
    }
}
