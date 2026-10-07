namespace MarketBook.Application.Features.Portfolios.RiskEvaluations.Common;

/// <summary>
/// Represents a persisted portfolio risk-evaluation snapshot.
/// </summary>
public sealed record PortfolioRiskEvaluationResponse
{
    /// <summary>Initializes a new persisted portfolio risk-evaluation response.</summary>
    public PortfolioRiskEvaluationResponse(
        string id,
        string portfolioId,
        string baseCurrencyId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval,
        decimal confidenceLevel,
        decimal riskFreeRateAnnual,
        decimal minimumAcceptableReturnAnnual,
        DateTimeOffset policyAsOf,
        string limitSource,
        string? policyId,
        int? policyVersion,
        bool isComplete,
        string overallStatus,
        int configuredLimitCount,
        int breachedLimitCount,
        int notCalculableLimitCount,
        DateTimeOffset evaluatedOn,
        IReadOnlyCollection<PortfolioRiskEvaluationRuleResponse> rules)
    {
        Id = id;
        PortfolioId = portfolioId;
        BaseCurrencyId = baseCurrencyId;
        From = from;
        To = to;
        Interval = interval;
        ConfidenceLevel = confidenceLevel;
        RiskFreeRateAnnual = riskFreeRateAnnual;
        MinimumAcceptableReturnAnnual = minimumAcceptableReturnAnnual;
        PolicyAsOf = policyAsOf;
        LimitSource = limitSource;
        PolicyId = policyId;
        PolicyVersion = policyVersion;
        IsComplete = isComplete;
        OverallStatus = overallStatus;
        ConfiguredLimitCount = configuredLimitCount;
        BreachedLimitCount = breachedLimitCount;
        NotCalculableLimitCount = notCalculableLimitCount;
        EvaluatedOn = evaluatedOn;
        Rules = rules;
    }

    /// <summary>Gets the evaluation identifier.</summary>
    public string Id { get; }

    /// <summary>Gets the portfolio identifier.</summary>
    public string PortfolioId { get; }

    /// <summary>Gets the base-currency identifier captured by the snapshot.</summary>
    public string BaseCurrencyId { get; }

    /// <summary>Gets the evaluation period start.</summary>
    public DateTimeOffset From { get; }

    /// <summary>Gets the evaluation period end.</summary>
    public DateTimeOffset To { get; }

    /// <summary>Gets the performance interval.</summary>
    public string Interval { get; }

    /// <summary>Gets the confidence level used by risk calculations.</summary>
    public decimal ConfidenceLevel { get; }

    /// <summary>Gets the annual risk-free rate used by risk calculations.</summary>
    public decimal RiskFreeRateAnnual { get; }

    /// <summary>Gets the annual minimum acceptable return used by risk calculations.</summary>
    public decimal MinimumAcceptableReturnAnnual { get; }

    /// <summary>Gets the policy-effective timestamp used for the evaluation.</summary>
    public DateTimeOffset PolicyAsOf { get; }

    /// <summary>Gets the source from which limits were resolved.</summary>
    public string LimitSource { get; }

    /// <summary>Gets the persisted policy identifier, when applicable.</summary>
    public string? PolicyId { get; }

    /// <summary>Gets the persisted policy version, when applicable.</summary>
    public int? PolicyVersion { get; }

    /// <summary>Gets whether the underlying risk calculation was complete.</summary>
    public bool IsComplete { get; }

    /// <summary>Gets the overall risk-limit evaluation status.</summary>
    public string OverallStatus { get; }

    /// <summary>Gets the number of configured limits.</summary>
    public int ConfiguredLimitCount { get; }

    /// <summary>Gets the number of breached limits.</summary>
    public int BreachedLimitCount { get; }

    /// <summary>Gets the number of configured limits that could not be calculated.</summary>
    public int NotCalculableLimitCount { get; }

    /// <summary>Gets the timestamp at which the snapshot was persisted.</summary>
    public DateTimeOffset EvaluatedOn { get; }

    /// <summary>Gets the persisted rule-evaluation snapshots.</summary>
    public IReadOnlyCollection<PortfolioRiskEvaluationRuleResponse> Rules { get; }
}

/// <summary>
/// Represents one persisted risk-limit rule result.
/// </summary>
public sealed record PortfolioRiskEvaluationRuleResponse
{
    /// <summary>Initializes a new persisted risk-limit rule response.</summary>
    public PortfolioRiskEvaluationRuleResponse(
        string code,
        string direction,
        bool isConfigured,
        string status,
        decimal? limit,
        decimal? actual,
        decimal? breachAmount)
    {
        Code = code;
        Direction = direction;
        IsConfigured = isConfigured;
        Status = status;
        Limit = limit;
        Actual = actual;
        BreachAmount = breachAmount;
    }

    /// <summary>Gets the rule code.</summary>
    public string Code { get; }

    /// <summary>Gets the comparison direction.</summary>
    public string Direction { get; }

    /// <summary>Gets whether the rule had a configured limit.</summary>
    public bool IsConfigured { get; }

    /// <summary>Gets the rule evaluation status.</summary>
    public string Status { get; }

    /// <summary>Gets the configured limit.</summary>
    public decimal? Limit { get; }

    /// <summary>Gets the calculated actual value.</summary>
    public decimal? Actual { get; }

    /// <summary>Gets the amount by which the rule was breached.</summary>
    public decimal? BreachAmount { get; }
}
