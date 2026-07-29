// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
// -----------------------------------------------------------------------------

using MarketBook.Tools.ProjectInspector.Models;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Checks XML documentation.
/// FA: مستندات XML را بررسی می‌کند.
/// </summary>
public sealed class DocumentationAnalyzer
{
    public void Analyze(ProjectReport report)
    {
        if (report.TotalClasses == 0)
        {
            report.DocumentationScore = 100;
            return;
        }

        var documented =
            report.Classes.Count(x => x.HasXmlDocumentation);

        report.DocumentationScore =
            documented * 100 / report.TotalClasses;
    }
}