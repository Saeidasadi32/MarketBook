// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
// -----------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;
using MarketBook.Tools.ProjectInspector.Models;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Analyzes C# classes.
/// FA: کلاس‌های سی‌شارپ را تحلیل می‌کند.
/// </summary>
public sealed class ClassAnalyzer
{
    /// <summary>
    /// EN: Analyzes one class.
    /// FA: یک کلاس را تحلیل می‌کند.
    /// </summary>
    public ProjectClass Analyze(ClassDeclarationSyntax node)
    {
        return new ProjectClass
        {
            Name = node.Identifier.Text,
            IsPublic = node.Modifiers.Any(x => x.Text == "public"),
            IsAbstract = node.Modifiers.Any(x => x.Text == "abstract"),
            IsSealed = node.Modifiers.Any(x => x.Text == "sealed"),
            ConstructorCount = node.Members.OfType<ConstructorDeclarationSyntax>().Count(),
            MethodCount = node.Members.OfType<MethodDeclarationSyntax>().Count(),
            PropertyCount = node.Members.OfType<PropertyDeclarationSyntax>().Count(),
            FieldCount = node.Members.OfType<FieldDeclarationSyntax>().Count(),
            Kind = "Class"
        };
    }
}