// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk;

/// <summary>
/// EN: Shared deterministic historical-simulation VaR/CVaR calculator.
/// FA: محاسبه‌گر مشترک و قطعی VaR/CVaR با روش شبیه‌سازی تاریخی.
/// </summary>
internal static class HistoricalValueAtRiskCalculator
{
    /// <summary>
    /// EN: Calculates the DOC-0037 nearest-rank historical VaR/CVaR convention.
    /// FA: قرارداد VaR/CVaR تاریخی DOC-0037 را با روش nearest-rank محاسبه می‌کند.
    /// </summary>
    /// <param name="returns">EN: Periodic returns. FA: بازده‌های دوره‌ای.</param>
    /// <param name="confidenceLevel">EN: Confidence level in the open interval (0,1). FA: سطح اطمینان در بازه باز (0,1).</param>
    /// <returns>EN: Deterministic VaR/CVaR calculation. FA: نتیجه قطعی محاسبه VaR/CVaR.</returns>
    internal static HistoricalValueAtRiskCalculation Calculate(
        IEnumerable<decimal> returns,
        decimal confidenceLevel)
    {
        ArgumentNullException.ThrowIfNull(returns);

        decimal[] sortedReturns =
            returns
                .OrderBy(item => item)
                .ToArray();

        if (sortedReturns.Length == 0)
        {
            throw new ArgumentException(
                "At least one return is required.",
                nameof(returns));
        }

        decimal tailProbability =
            1m - confidenceLevel;

        int quantileRank =
            (int)Math.Ceiling(
                (double)(tailProbability * sortedReturns.Length));

        quantileRank =
            Math.Clamp(
                quantileRank,
                1,
                sortedReturns.Length);

        decimal quantileReturn =
            sortedReturns[quantileRank - 1];

        decimal valueAtRiskReturn =
            Math.Max(
                -quantileReturn,
                0m);

        decimal[] tailReturns =
            sortedReturns
                .Where(item => item <= quantileReturn)
                .ToArray();

        decimal averageTailReturn =
            tailReturns.Average();

        decimal conditionalValueAtRiskReturn =
            Math.Max(
                -averageTailReturn,
                0m);

        return new HistoricalValueAtRiskCalculation(
            sortedReturns,
            quantileReturn,
            valueAtRiskReturn,
            conditionalValueAtRiskReturn,
            tailReturns.Length);
    }
}

/// <summary>
/// EN: Result of the shared historical VaR/CVaR calculation.
/// FA: نتیجه محاسبه مشترک VaR/CVaR تاریخی.
/// </summary>
/// <param name="SortedReturns">EN: Ascending periodic returns. FA: بازده‌های دوره‌ای مرتب‌شده صعودی.</param>
/// <param name="HistoricalQuantileReturn">EN: Raw nearest-rank quantile return. FA: صدک خام nearest-rank بازده.</param>
/// <param name="ValueAtRiskReturn">EN: Positive VaR loss ratio. FA: نسبت زیان مثبت VaR.</param>
/// <param name="ConditionalValueAtRiskReturn">EN: Positive CVaR loss ratio. FA: نسبت زیان مثبت CVaR.</param>
/// <param name="TailObservationCount">EN: Number of returns at or below the VaR quantile. FA: تعداد بازده‌ها در صدک VaR یا پایین‌تر از آن.</param>
internal sealed record HistoricalValueAtRiskCalculation(
    IReadOnlyCollection<decimal> SortedReturns,
    decimal HistoricalQuantileReturn,
    decimal ValueAtRiskReturn,
    decimal ConditionalValueAtRiskReturn,
    int TailObservationCount);
