// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
// -----------------------------------------------------------------------------

using MarketBook.Tools.ProjectInspector.Models;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Calculates architecture score.
/// FA: امتیاز معماری پروژه را محاسبه می‌کند.
/// </summary>
public sealed class ArchitectureAnalyzer
{
    public void Analyze(ProjectReport report)
    {
        var score = 100;

        score -= report.Issues.Sum(x => x.Severity);

        report.ArchitectureScore =
            Math.Max(score, 0);

        report.DddScore =
            report.ArchitectureScore;
    }
}