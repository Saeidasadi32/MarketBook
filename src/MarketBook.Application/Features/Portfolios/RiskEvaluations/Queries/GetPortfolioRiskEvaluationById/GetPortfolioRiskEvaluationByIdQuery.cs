using MediatR;
using MarketBook.Application.Features.Portfolios.RiskEvaluations.Common;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Portfolios.RiskEvaluations.Queries.GetPortfolioRiskEvaluationById;

/// <summary>
/// Requests a persisted portfolio risk-evaluation snapshot by identifier.
/// </summary>
public sealed record GetPortfolioRiskEvaluationByIdQuery
    : IRequest<Result<PortfolioRiskEvaluationResponse>>
{
    /// <summary>Initializes a new risk-evaluation lookup query.</summary>
    public GetPortfolioRiskEvaluationByIdQuery(
        string portfolioId,
        string evaluationId)
    {
        PortfolioId = portfolioId;
        EvaluationId = evaluationId;
    }

    /// <summary>Gets the portfolio identifier.</summary>
    public string PortfolioId { get; }

    /// <summary>Gets the risk-evaluation identifier.</summary>
    public string EvaluationId { get; }
}
