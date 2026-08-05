// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
// -----------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Loads Visual Studio solutions.
/// FA: فایل Solution را بارگذاری می‌کند.
/// </summary>
public sealed class SolutionLoader
{
    /// <summary>
    /// EN: Loads a solution.
    /// FA: یک Solution را بارگذاری می‌کند.
    /// </summary>
    public async Task<Solution> LoadAsync(string solutionPath)
    {
        using var workspace = RoslynWorkspaceFactory.Create();

        return await workspace.OpenSolutionAsync(solutionPath);
    }
}