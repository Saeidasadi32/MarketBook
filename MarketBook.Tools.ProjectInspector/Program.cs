// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tools
// Namespace : MarketBook.Tools.ProjectInspector
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Tools.ProjectInspector.Services;

Console.WriteLine("========================================");
Console.WriteLine(" MarketBook Project Inspector");
Console.WriteLine("========================================");
Console.WriteLine();

var root = args.Length > 0
    ? args[0]
    : Directory.GetCurrentDirectory();

var scanner = new ProjectScanner();

var report = scanner.Scan(root);

//MarkdownWriter.Write(report, Path.Combine(root, "ProjectReport.md"));

Console.WriteLine("Done.");