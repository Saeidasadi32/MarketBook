// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
// -----------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Reads namespaces.
/// FA: Namespaceها را استخراج می‌کند.
/// </summary>
public sealed class NamespaceAnalyzer
{
    public string Analyze(CompilationUnitSyntax root)
    {
        var ns = root.Members
                     .OfType<BaseNamespaceDeclarationSyntax>()
                     .FirstOrDefault();

        return ns?.Name.ToString() ?? "";
    }
}