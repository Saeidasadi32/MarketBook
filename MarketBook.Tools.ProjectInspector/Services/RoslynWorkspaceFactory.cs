// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Services
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace MarketBook.Tools.ProjectInspector.Services;

/// <summary>
/// EN: Creates an MSBuild workspace.
/// FA: Workspace مربوط به MSBuild را ایجاد می‌کند.
/// </summary>
public static class RoslynWorkspaceFactory
{
    /// <summary>
    /// EN: Creates a workspace.
    /// FA: یک Workspace ایجاد می‌کند.
    /// </summary>
    public static MSBuildWorkspace Create()
    {
        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }

        return MSBuildWorkspace.Create();
    }
}