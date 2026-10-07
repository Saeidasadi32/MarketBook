namespace MarketBook.Api.Controllers;

/// <summary>Request body for creating a persisted portfolio risk-evaluation snapshot.</summary>
public sealed record CreatePortfolioRiskEvaluationRequest
{
    /// <summary>Evaluation period start.</summary>
    public DateTimeOffset From { get; init; }
    /// <summary>Evaluation period end.</summary>
    public DateTimeOffset To { get; init; }
    /// <summary>Performance interval.</summary>
    public string Interval { get; init; } = "Daily";
    /// <summary>Confidence level.</summary>
    public decimal ConfidenceLevel { get; init; } = 0.95m;
    /// <summary>Annual risk-free rate.</summary>
    public decimal RiskFreeRateAnnual { get; init; }
    /// <summary>Annual minimum acceptable return.</summary>
    public decimal MinimumAcceptableReturnAnnual { get; init; }
    /// <summary>Optional maximum annualized volatility.</summary>
    public decimal? MaxAnnualizedVolatility { get; init; }
    /// <summary>Optional maximum VaR return.</summary>
    public decimal? MaxValueAtRiskReturn { get; init; }
    /// <summary>Optional maximum VaR amount in base currency.</summary>
    public decimal? MaxValueAtRiskAmountBase { get; init; }
    /// <summary>Optional maximum drawdown loss ratio.</summary>
    public decimal? MaxDrawdownLossRatio { get; init; }
    /// <summary>Optional maximum drawdown amount in base currency.</summary>
    public decimal? MaxDrawdownAmountBase { get; init; }
    /// <summary>Optional minimum Sharpe ratio.</summary>
    public decimal? MinSharpeRatio { get; init; }
    /// <summary>Optional minimum Sortino ratio.</summary>
    public decimal? MinSortinoRatio { get; init; }
}
