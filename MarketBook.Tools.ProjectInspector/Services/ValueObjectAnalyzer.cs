// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
// -----------------------------------------------------------------------------

using MarketBook.Tools.ProjectInspector.Models;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Detects Value Objects.
/// FA: Value Objectها را شناسایی می‌کند.
/// </summary>
public sealed class ValueObjectAnalyzer
{
    public void Analyze(ProjectReport report)
    {
        foreach (var type in report.Classes)
        {
            if (type.Kind == "Record")
            {
                report.TotalValueObjects++;
            }
        }
    }
}