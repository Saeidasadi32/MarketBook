// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.PortfolioRiskPolicy.Enums
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.PortfolioRiskPolicy.Enums;

/// <summary>
/// EN: Lifecycle status of a persisted portfolio risk policy version.
/// FA: وضعیت چرخه عمر نسخه ذخیره‌شده سیاست ریسک پرتفوی.
/// </summary>
public enum RiskPolicyStatus
{
    /// <summary>EN: Editable/inactive policy version. FA: نسخه غیرفعال/پیش‌نویس.</summary>
    Draft = 1,

    /// <summary>EN: Currently effective policy version. FA: نسخه فعال فعلی.</summary>
    Active = 2,

    /// <summary>EN: Historical policy version. FA: نسخه تاریخی بایگانی‌شده.</summary>
    Archived = 3
}
