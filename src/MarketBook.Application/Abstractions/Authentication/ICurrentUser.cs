// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Authentication
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Abstractions.Authentication;

/// <summary>
/// EN: Provides information about the current authenticated user.
/// FA: اطلاعات کاربر احراز هویت شده جاری را فراهم می‌کند.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// EN: Gets current user identifier.
    /// FA: شناسه کاربر جاری.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// EN: Gets current user name.
    /// FA: نام کاربر جاری.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// EN: Gets whether current user is authenticated.
    /// FA: مشخص می‌کند کاربر احراز هویت شده است یا خیر.
    /// </summary>
    bool IsAuthenticated { get; }
}