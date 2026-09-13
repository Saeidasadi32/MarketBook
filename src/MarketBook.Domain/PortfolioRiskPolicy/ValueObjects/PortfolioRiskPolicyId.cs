// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.PortfolioRiskPolicy.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.PortfolioRiskPolicy.ValueObjects;

/// <summary>
/// EN: Identifies a persisted portfolio risk policy version.
/// FA: یک نسخه ذخیره‌شده از سیاست ریسک پرتفوی را شناسایی می‌کند.
/// </summary>
public sealed record PortfolioRiskPolicyId : EntityId
{
    private PortfolioRiskPolicyId(Ulid value) : base(value) { }

    /// <summary>EN: Creates a new identifier. FA: شناسه جدید ایجاد می‌کند.</summary>
    public static PortfolioRiskPolicyId New() => new(Ulid.NewUlid());

    /// <summary>EN: Parses an identifier. FA: شناسه را Parse می‌کند.</summary>
    public static PortfolioRiskPolicyId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(PortfolioRiskPolicyId));
        if (Ulid.TryParse(value, out Ulid ulid))
        {
            return new PortfolioRiskPolicyId(ulid);
        }

        throw new DomainException(
            new Error("PortfolioRiskPolicyId.InvalidFormat", "The risk-policy identifier is invalid."));
    }
}
