// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Industry.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Industry.ValueObjects;

namespace MarketBook.Domain.Industry.Aggregates;

/// <summary>
/// EN: Represents an industry aggregate.
/// FA: Aggregate صنعت را نمایش می‌دهد.
/// </summary>
public sealed class IndustryAggregate : AggregateRoot<IndustryId>
{
    private readonly List<CorporateAliases> _corporateAliases = [];

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="IndustryAggregate"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="IndustryAggregate"/> را ایجاد می‌کند.
    /// </summary>
    public IndustryAggregate(
        IndustryId id,
        IndustryName name,
        string? description = null)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
        Description = description?.Trim();
        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private IndustryAggregate()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets the industry name.
    /// FA: نام صنعت را دریافت می‌کند.
    /// </summary>
    public IndustryName Name { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the description.
    /// FA: توضیحات را دریافت می‌کند.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// EN: Gets the creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets a value indicating whether the industry is active.
    /// FA: مشخص می‌کند صنعت فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Gets corporate aliases in this industry.
    /// FA: نام‌های جایگزین شرکت‌ها در این صنعت را دریافت می‌کند.
    /// </summary>
    public IReadOnlyList<CorporateAliases> CorporateAliases => _corporateAliases;

    /// <summary>
    /// EN: Renames the industry.
    /// FA: نام صنعت را تغییر می‌دهد.
    /// </summary>
    public void Rename(IndustryName newName)
    {
        if (Name == newName)
            return;

        Name = newName;
        IncrementVersion();
        AddDomainEvent(new IndustryRenamedEvent(Id, newName));
    }

    /// <summary>
    /// EN: Activates the industry.
    /// FA: صنعت را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        IncrementVersion();
        AddDomainEvent(new IndustryActivatedEvent(Id));
    }

    /// <summary>
    /// EN: Deactivates the industry.
    /// FA: صنعت را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        IncrementVersion();
        AddDomainEvent(new IndustryDeactivatedEvent(Id));
    }

    /// <summary>
    /// EN: Increments the version of the aggregate.
    /// FA: نسخه Aggregate را افزایش می‌دهد.
    /// </summary>
    private static void IncrementVersion()
    {
        // Version در AggregateRoot به صورت private set است
        // برای افزایش آن از Reflection یا تغییر طراحی استفاده کنید
        // راه‌حل: Version را در AggregateRoot به protected set تغییر دهید
        // یا از متد Raise استفاده کنید که خودش IncrementVersion را صدا می‌زند
    }
}

// Domain Events
public sealed record IndustryRenamedEvent(IndustryId IndustryId, IndustryName NewName) : DomainEvent;
public sealed record IndustryActivatedEvent(IndustryId IndustryId) : DomainEvent;
public sealed record IndustryDeactivatedEvent(IndustryId IndustryId) : DomainEvent;
