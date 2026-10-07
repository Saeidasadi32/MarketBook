// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.PortfolioRiskEvaluation.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;

namespace MarketBook.Domain.PortfolioRiskEvaluation.Entities;

/// <summary>
/// EN: Immutable-at-aggregate-level snapshot of one resolved risk-limit rule.
/// FA: Snapshot ÛŒÚ© Rule Ø­Ù„â€ŒØ´Ø¯Ù‡ Ø§Ø² Ø­Ø¯ÙˆØ¯ Ø±ÛŒØ³Ú© Ø¯Ø± Ø²Ù…Ø§Ù† Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒ.
/// </summary>
public sealed class PortfolioRiskEvaluationRule
{
    /// <summary>
    /// EN: Creates one persisted rule snapshot.
    /// FA: ÛŒÚ© Snapshot Ø§Ø² Rule Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒâ€ŒØ´Ø¯Ù‡ Ø§ÛŒØ¬Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
    /// </summary>
    public PortfolioRiskEvaluationRule(
        string code,
        string direction,
        bool isConfigured,
        string status,
        decimal? limit,
        decimal? actual,
        decimal? breachAmount)
    {
        Guard.AgainstNullOrWhiteSpace(code, nameof(code));
        Guard.AgainstNullOrWhiteSpace(direction, nameof(direction));
        Guard.AgainstNullOrWhiteSpace(status, nameof(status));

        Code = code.Trim();
        Direction = direction.Trim();
        IsConfigured = isConfigured;
        Status = status.Trim();
        Limit = limit;
        Actual = actual;
        BreachAmount = breachAmount;
    }

    private PortfolioRiskEvaluationRule()
    {
        Code = string.Empty;
        Direction = string.Empty;
        Status = string.Empty;
    }

    /// <summary>EN: Stable rule code. FA: Ú©Ø¯ Ù¾Ø§ÛŒØ¯Ø§Ø± Rule.</summary>
    public string Code { get; private set; }

    /// <summary>EN: Maximum or Minimum. FA: Ø¬Ù‡Øª Ø¢Ø³ØªØ§Ù†Ù‡ Maximum ÛŒØ§ Minimum.</summary>
    public string Direction { get; private set; }

    /// <summary>EN: Whether this rule was configured. FA: Ù…Ø´Ø®Øµ Ù…ÛŒâ€ŒÚ©Ù†Ø¯ Rule Ù¾ÛŒÚ©Ø±Ø¨Ù†Ø¯ÛŒ Ø´Ø¯Ù‡ Ø§Ø³Øª ÛŒØ§ Ø®ÛŒØ±.</summary>
    public bool IsConfigured { get; private set; }

    /// <summary>EN: Evaluation status. FA: ÙˆØ¶Ø¹ÛŒØª Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒ Rule.</summary>
    public string Status { get; private set; }

    /// <summary>EN: Resolved limit threshold. FA: Ø¢Ø³ØªØ§Ù†Ù‡ Ù†Ù‡Ø§ÛŒÛŒ Ø­Ù„â€ŒØ´Ø¯Ù‡.</summary>
    public decimal? Limit { get; private set; }

    /// <summary>EN: Actual evaluated metric. FA: Ù…Ù‚Ø¯Ø§Ø± ÙˆØ§Ù‚Ø¹ÛŒ Metric Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒâ€ŒØ´Ø¯Ù‡.</summary>
    public decimal? Actual { get; private set; }

    /// <summary>EN: Positive breach amount when breached. FA: Ù…ÛŒØ²Ø§Ù† Ø¹Ø¨ÙˆØ± Ù…Ø«Ø¨Øª Ø§Ø² Ø­Ø¯ Ø¯Ø± ØµÙˆØ±Øª Breach.</summary>
    public decimal? BreachAmount { get; private set; }
}