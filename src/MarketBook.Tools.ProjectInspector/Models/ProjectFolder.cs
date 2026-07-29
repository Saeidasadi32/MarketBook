// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector.Models
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Tools.ProjectInspector.Models;

/// <summary>
/// EN: Represents a physical folder.
/// FA: یک پوشه پروژه را نمایش می‌دهد.
/// </summary>
public sealed class ProjectFolder
{
    /// <summary>
    /// EN: Folder path.
    /// FA: مسیر پوشه.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// EN: Child files.
    /// FA: فایل‌های داخل پوشه.
    /// </summary>
    public List<ProjectFile> Files { get; } = [];

    /// <summary>
    /// EN: Child folders.
    /// FA: زیرپوشه‌ها.
    /// </summary>
    public List<ProjectFolder> Children { get; } = [];
}