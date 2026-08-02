// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Localization
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Abstractions.Localization;

/// <summary>
/// EN: Provides localized strings.
/// FA: رشته‌های محلی‌سازی شده را فراهم می‌کند.
/// </summary>
public interface IStringLocalizer
{
    /// <summary>
    /// EN: Gets localized text.
    /// FA: متن محلی‌سازی شده را دریافت می‌کند.
    /// </summary>
    string this[string key] { get; }

    /// <summary>
    /// EN: Gets localized text with arguments.
    /// FA: متن محلی‌سازی شده همراه با پارامترها را دریافت می‌کند.
    /// </summary>
    string this[string key, params object[] arguments] { get; }
}