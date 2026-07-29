// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Models
// -----------------------------------------------------------------------------

namespace MarketBook.Tools.ProjectInspector.Models;

/// <summary>
/// EN: Represents an architecture issue.
/// FA: یک مشکل معماری را نمایش می‌دهد.
/// </summary>
public sealed class ProjectIssue
{
    public required string Category { get; init; }

    public required string File { get; init; }

    public required string Description { get; init; }

    public required string Recommendation { get; init; }

    public int Severity { get; init; }
}