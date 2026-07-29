// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Models
// -----------------------------------------------------------------------------

namespace MarketBook.Tools.ProjectInspector.Models;

/// <summary>
/// EN: Represents a C# type.
/// FA: یک نوع سی‌شارپ را نمایش می‌دهد.
/// </summary>
public sealed class ProjectClass
{
    public required string Name { get; init; }

    public string Namespace { get; init; } = string.Empty;

    public string Kind { get; init; } = string.Empty;

    public bool IsPublic { get; init; }

    public bool IsAbstract { get; init; }

    public bool IsSealed { get; init; }

    public bool HasXmlDocumentation { get; set; }

    public int MethodCount { get; set; }

    public int PropertyCount { get; set; }

    public int FieldCount { get; set; }

    public int ConstructorCount { get; set; }

    public int LineCount { get; set; }

    public List<string> BaseTypes { get; } = [];

    public List<string> Interfaces { get; } = [];

    public List<string> Usings { get; } = [];
}