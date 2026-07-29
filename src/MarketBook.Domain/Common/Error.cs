// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Represents a domain error.
/// FA: یک خطای دامنه را نمایش می‌دهد.
/// </summary>
public sealed record Error
{
    /// <summary>
    /// EN: Represents no error.
    /// FA: بدون خطا.
    /// </summary>
    public static readonly Error None = new(
        string.Empty,
        string.Empty);

    /// <summary>
    /// EN: Initializes a new error.
    /// FA: یک خطای جدید ایجاد می‌کند.
    /// </summary>
    public Error(
        string code,
        string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code.Trim();
        Message = message.Trim();
    }

    /// <summary>
    /// EN: Gets error code.
    /// FA: کد خطا.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// EN: Gets error message.
    /// FA: متن خطا.
    /// </summary>
    public string Message { get; }

    /// <inheritdoc/>
    public override string ToString()
        => $"{Code}: {Message}";
}