// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
// -----------------------------------------------------------------------------

using MarketBook.Tools.ProjectInspector.Models;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Detects DDD Entities.
/// FA: Entityهای DDD را شناسایی می‌کند.
/// </summary>
public sealed class EntityAnalyzer
{
    public void Analyze(ProjectReport report)
    {
        foreach (var type in report.Classes)
        {
            if (type.BaseTypes.Contains("Entity"))
            {
                report.TotalEntities++;
            }
        }
    }
}