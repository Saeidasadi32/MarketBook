// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Tools.ProjectInspector.Models;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Detects DDD Aggregates.
/// FA: Aggregateهای DDD را شناسایی می‌کند.
/// </summary>
public sealed class AggregateAnalyzer
{
    /// <summary>
    /// EN: Analyzes aggregate classes.
    /// FA: کلاس‌های Aggregate را تحلیل می‌کند.
    /// </summary>
    public void Analyze(ProjectReport report)
    {
        foreach (var type in report.Classes)
        {
            if (type.BaseTypes.Contains("AggregateRoot"))
            {
                report.TotalAggregates++;
            }
        }
    }
}