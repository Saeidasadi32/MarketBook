// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Models
// -----------------------------------------------------------------------------

namespace MarketBook.Tools.ProjectInspector.Models;

/// <summary>
/// EN: Represents a source file.
/// FA: یک فایل سورس را نمایش می‌دهد.
/// </summary>
public sealed class ProjectFile
{
    public required string FileName { get; init; }

    public required string FullPath { get; init; }

    public string Namespace { get; set; } = string.Empty;

    public List<ProjectClass> Classes { get; } = [];
}