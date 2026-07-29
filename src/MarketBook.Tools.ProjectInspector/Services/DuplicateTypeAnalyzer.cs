// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
// -----------------------------------------------------------------------------

using MarketBook.Tools.ProjectInspector.Models;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Finds duplicated types.
/// FA: کلاس‌های تکراری را پیدا می‌کند.
/// </summary>
public sealed class DuplicateTypeAnalyzer
{
    public void Analyze(ProjectReport report)
    {
        var duplicates =
            report.Classes
                  .GroupBy(x => x.Name)
                  .Where(x => x.Count() > 1);

        foreach (var duplicate in duplicates)
        {
            report.Issues.Add(new ProjectIssue
            {
                Category = "Duplicate Type",
                File = "-",
                Severity = 5,
                Description =
                    $"Duplicated type '{duplicate.Key}' detected.",

                Recommendation =
                    "Merge or rename duplicated classes."
            });
        }
    }
}