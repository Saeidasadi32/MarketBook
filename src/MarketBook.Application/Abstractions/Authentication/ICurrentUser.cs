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
    /// EN: Gets the current user identifier.
    /// FA: شناسه کاربر جاری را دریافت می‌کند.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// EN: Gets the current user name.
    /// FA: نام کاربر جاری را دریافت می‌کند.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// EN: Gets whether the current user is authenticated.
    /// FA: مشخص می‌کند آیا کاربر جاری احراز هویت شده است یا خیر.
    /// </summary>
    bool IsAuthenticated { get; }
}
