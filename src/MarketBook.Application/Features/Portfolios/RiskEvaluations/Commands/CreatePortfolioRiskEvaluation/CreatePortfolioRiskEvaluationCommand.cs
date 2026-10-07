using MediatR;
using MarketBook.Application.Features.Portfolios.RiskEvaluations.Common;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Portfolios.RiskEvaluations.Commands.CreatePortfolioRiskEvaluation;

/// <summary>
/// Requests calculation and persistence of an official portfolio risk-evaluation snapshot.
/// </summary>
public sealed record CreatePortfolioRiskEvaluationCommand
    : IRequest<Result<PortfolioRiskEvaluationResponse>>
{
    /// <summary>Initializes a new create-risk-evaluation command.</summary>
    public CreatePortfolioRiskEvaluationCommand(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval,
        decimal confidenceLevel = 0.95m,
        decimal riskFreeRateAnnual = 0m,
        decimal minimumAcceptableReturnAnnual = 0m,
        decimal? maxAnnualizedVolatility = null,
        decimal? maxValueAtRiskReturn = null,
        decimal? maxValueAtRiskAmountBase = null,
        decimal? maxDrawdownLossRatio = null,
        decimal? maxDrawdownAmountBase = null,
        decimal? minSharpeRatio = null,
        decimal? minSortinoRatio = null)
    {
        PortfolioId = portfolioId;
        From = from;
        To = to;
        Interval = interval;
        ConfidenceLevel = confidenceLevel;
        RiskFreeRateAnnual = riskFreeRateAnnual;
        MinimumAcceptableReturnAnnual = minimumAcceptableReturnAnnual;
        MaxAnnualizedVolatility = maxAnnualizedVolatility;
        MaxValueAtRiskReturn = maxValueAtRiskReturn;
        MaxValueAtRiskAmountBase = maxValueAtRiskAmountBase;
        MaxDrawdownLossRatio = maxDrawdownLossRatio;
        MaxDrawdownAmountBase = maxDrawdownAmountBase;
        MinSharpeRatio = minSharpeRatio;
        MinSortinoRatio = minSortinoRatio;
    }

    /// <summary>Gets the portfolio identifier.</summary>
    public string PortfolioId { get; }

    /// <summary>Gets the evaluation period start.</summary>
    public DateTimeOffset From { get; }

    /// <summary>Gets the evaluation period end.</summary>
    public DateTimeOffset To { get; }

    /// <summary>Gets the performance interval.</summary>
    public string Interval { get; }

    /// <summary>Gets the confidence level.</summary>
    public decimal ConfidenceLevel { get; }

    /// <summary>Gets the annual risk-free rate.</summary>
    public decimal RiskFreeRateAnnual { get; }

    /// <summary>Gets the annual minimum acceptable return.</summary>
    public decimal MinimumAcceptableReturnAnnual { get; }

    /// <summary>Gets the optional maximum annualized-volatility override.</summary>
    public decimal? MaxAnnualizedVolatility { get; }

    /// <summary>Gets the optional maximum value-at-risk return override.</summary>
    public decimal? MaxValueAtRiskReturn { get; }

    /// <summary>Gets the optional maximum value-at-risk amount override.</summary>
    public decimal? MaxValueAtRiskAmountBase { get; }

    /// <summary>Gets the optional maximum drawdown-loss-ratio override.</summary>
    public decimal? MaxDrawdownLossRatio { get; }

    /// <summary>Gets the optional maximum drawdown amount override.</summary>
    public decimal? MaxDrawdownAmountBase { get; }

    /// <summary>Gets the optional minimum Sharpe-ratio override.</summary>
    public decimal? MinSharpeRatio { get; }

    /// <summary>Gets the optional minimum Sortino-ratio override.</summary>
    public decimal? MinSortinoRatio { get; }
}
