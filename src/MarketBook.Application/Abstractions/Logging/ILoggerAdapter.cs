// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Abstractions.Logging
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Abstractions.Logging;

/// <summary>
/// EN: Defines application logging operations.
/// FA: عملیات لاگ برنامه را تعریف می‌کند.
/// </summary>
/// <typeparam name="T">
/// EN: Type associated with the logger.
/// FA: نوع مرتبط با لاگر.
/// </typeparam>
public interface ILoggerAdapter<T>
{
    /// <summary>
    /// EN: Writes an informational log entry.
    /// FA: یک پیام اطلاعاتی را ثبت می‌کند.
    /// </summary>
    /// <param name="message">
    /// EN: Log message.
    /// FA: متن پیام.
    /// </param>
    /// <param name="args">
    /// EN: Message formatting arguments.
    /// FA: آرگومان‌های قالب‌بندی پیام.
    /// </param>
    void LogInformation(
        string message,
        params object?[] args);

    /// <summary>
    /// EN: Writes a warning log entry.
    /// FA: یک پیام هشدار را ثبت می‌کند.
    /// </summary>
    /// <param name="message">
    /// EN: Log message.
    /// FA: متن پیام.
    /// </param>
    /// <param name="args">
    /// EN: Message formatting arguments.
    /// FA: آرگومان‌های قالب‌بندی پیام.
    /// </param>
    void LogWarning(
        string message,
        params object?[] args);

    /// <summary>
    /// EN: Writes an error log entry.
    /// FA: یک پیام خطا را ثبت می‌کند.
    /// </summary>
    /// <param name="exception">
    /// EN: Exception to log.
    /// FA: استثنای ثبت‌شونده.
    /// </param>
    /// <param name="message">
    /// EN: Log message.
    /// FA: متن پیام.
    /// </param>
    /// <param name="args">
    /// EN: Message formatting arguments.
    /// FA: آرگومان‌های قالب‌بندی پیام.
    /// </param>
    void LogError(
        Exception exception,
        string message,
        params object?[] args);
}
