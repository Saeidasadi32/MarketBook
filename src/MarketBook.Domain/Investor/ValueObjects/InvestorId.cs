// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Investor.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.Investor.ValueObjects;

/// <summary>
/// EN: Represents the unique identifier of a Investor.
/// FA: شناسه یکتای Investor را نمایش می‌دهد.
/// </summary>
public sealed record InvestorId : EntityId
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="InvestorId"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="InvestorId"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="value">Underlying ULID value.</param>
    private InvestorId(Ulid value) : base(value)
    {
    }

    /// <summary>
    /// EN: Creates a new unique identifier.
    /// FA: یک شناسه یکتای جدید ایجاد می‌کند.
    /// </summary>
    public static InvestorId New() => new(Ulid.NewUlid());

    /// <summary>
    /// EN: Creates an identifier from an existing ULID.
    /// FA: یک شناسه از یک ULID موجود ایجاد می‌کند.
    /// </summary>
    public static InvestorId FromUlid(Ulid value) => new(value);

    /// <summary>
    /// EN: Parses a ULID string into an identifier.
    /// FA: رشته ULID را به شناسه تبدیل می‌کند.
    /// </summary>
    public static InvestorId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(InvestorId));

        if (Ulid.TryParse(value, out var ulid))
            return new(ulid);

        throw new DomainException(
            new Error(
                $"{nameof(InvestorId)}.InvalidFormat",
                $"'{value}' is not a valid ULID."));
    }

    /// <summary>
    /// EN: Attempts to parse a ULID string.
    /// FA: تلاش می‌کند رشته ULID را به شناسه تبدیل کند.
    /// </summary>
    public static bool TryParse(string? value, out InvestorId? result)
    {
        if (!string.IsNullOrWhiteSpace(value) && Ulid.TryParse(value, out var ulid))
        {
            result = new(ulid);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// EN: Explicit conversion from string.
    /// FA: تبدیل صریح از رشته.
    /// </summary>
    public static explicit operator InvestorId(string value) => Parse(value);

}