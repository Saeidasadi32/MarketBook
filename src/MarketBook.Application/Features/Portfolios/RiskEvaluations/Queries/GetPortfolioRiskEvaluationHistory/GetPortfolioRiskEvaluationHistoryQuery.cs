using MediatR;
using MarketBook.Application.Features.Portfolios.RiskEvaluations.Common;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Portfolios.RiskEvaluations.Queries.GetPortfolioRiskEvaluationHistory;

/// <summary>
/// Requests persisted risk-evaluation history for a portfolio.
/// </summary>
public sealed record GetPortfolioRiskEvaluationHistoryQuery
    : IRequest<Result<IReadOnlyCollection<PortfolioRiskEvaluationResponse>>>
{
    /// <summary>Initializes a new portfolio risk-evaluation history query.</summary>
    public GetPortfolioRiskEvaluationHistoryQuery(string portfolioId)
    {
        PortfolioId = portfolioId;
    }

    /// <summary>Gets the portfolio identifier.</summary>
    public string PortfolioId { get; }
}
