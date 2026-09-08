// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Investor.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.ValueObjects;

namespace MarketBook.Domain.Investor.Aggregates;

/// <summary>
/// EN: Represents an investor that owns one or more portfolios.
/// FA: سرمایه‌گذاری را نمایش می‌دهد که مالک یک یا چند پرتفوی است.
/// </summary>
public sealed class Investor : AggregateRoot<InvestorId>
{
    /// <summary>
    /// EN: Initializes a new investor.
    /// FA: یک سرمایه‌گذار جدید ایجاد می‌کند.
    /// </summary>
    public Investor(
        InvestorId id,
        string fullName)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

        FullName = fullName.Trim();
        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Investor()
    {
        FullName = default!;
    }

    /// <summary>EN: Gets the investor full name. FA: نام کامل سرمایه‌گذار را دریافت می‌کند.</summary>
    public string FullName { get; private set; }

    /// <summary>EN: Gets the creation timestamp. FA: زمان ایجاد را دریافت می‌کند.</summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>EN: Gets whether the investor is active. FA: فعال بودن سرمایه‌گذار را مشخص می‌کند.</summary>
    public bool IsActive { get; private set; }

    /// <summary>EN: Changes the investor full name. FA: نام کامل سرمایه‌گذار را تغییر می‌دهد.</summary>
    public void Rename(string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

        string normalizedName = fullName.Trim();

        if (FullName == normalizedName)
            return;

        FullName = normalizedName;
    }

    /// <summary>EN: Activates the investor. FA: سرمایه‌گذار را فعال می‌کند.</summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
    }

    /// <summary>EN: Deactivates the investor. FA: سرمایه‌گذار را غیرفعال می‌کند.</summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
    }

    /// <summary>EN: Creates an investor with a generated identifier. FA: سرمایه‌گذاری با شناسه تولیدشده ایجاد می‌کند.</summary>
    public static Investor Create(string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        return new Investor(InvestorId.New(), fullName);
    }
}
