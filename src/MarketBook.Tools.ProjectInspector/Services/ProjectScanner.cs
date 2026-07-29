// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
// -----------------------------------------------------------------------------

using MarketBook.Tools.ProjectInspector.Models;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Scans the project structure.
/// FA: ساختار پروژه را اسکن می‌کند.
/// </summary>
public sealed class ProjectScanner
{
    public ProjectReport Scan(string root)
    {
        var report = new ProjectReport
        {
            Root = root
        };

        foreach (var file in Directory.EnumerateFiles(
                     root,
                     "*.cs",
                     SearchOption.AllDirectories))
        {
            if (file.Contains("\\bin\\"))
                continue;

            if (file.Contains("\\obj\\"))
                continue;

            report.Files.Add(new ProjectFile
            {
                FileName = Path.GetFileName(file),
                FullPath = file
            });
        }

        return report;
    }
}