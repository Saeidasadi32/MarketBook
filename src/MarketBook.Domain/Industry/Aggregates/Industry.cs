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
public sealed class Industry : AggregateRoot<IndustryId>
{
    private readonly List<CorporateAliases> _corporateAliases = [];

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Industry"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Industry"/> را ایجاد می‌کند.
    /// </summary>
    public Industry(
        IndustryId id,
        IndustryCode code,
        IndustryName name,
        string? description = null)
        : base(id)
    {
        Guard.AgainstNull(name);

        Name = name;
        Code = code; 
        Description = description?.Trim();
        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Industry()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets the industry name.
    /// FA: نام صنعت را دریافت می‌کند.
    /// </summary>
    public IndustryName Name { get; private set; } = default!;

    public IndustryCode Code { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the description.
    /// FA: توضیحات را دریافت می‌کند.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// EN: Gets the creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// EN: Gets a value indicating whether the industry is active.
    /// FA: مشخص می‌کند صنعت فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Gets corporate aliases in this industry.
    /// FA: نام‌های جایگزین شرکت‌ها در این صنعت را دریافت می‌کند.
    /// </summary>
    public IReadOnlyCollection<CorporateAliases> CorporateAliases
    => _corporateAliases.AsReadOnly();

    /// <summary>
    /// EN: Renames the industry.
    /// FA: نام صنعت را تغییر می‌دهد.
    /// </summary>
    public void Rename(IndustryName newName)
    {
        if (Name == newName)
            return;

        Name = newName;
        Raise(new IndustryRenamedEvent(Id, newName));
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
        Raise(new IndustryActivatedEvent(Id));
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
        Raise(new IndustryDeactivatedEvent(Id));
    }

    public void ChangeDescription(string? description)
    {
        description = description?.Trim();

        if (Description == description)
            return;

        Description = description;

        Raise(new IndustryDescriptionChangedEvent(Id));
    }

    /// <summary>
    /// EN: Raised when the industry description changes.
    /// FA: هنگام تغییر توضیحات صنعت ایجاد می‌شود.
    /// </summary>


    public void AddAlias(CorporateAliases alias)
    {
        ArgumentNullException.ThrowIfNull(alias);

        if (_corporateAliases.Contains(alias))
            return;

        _corporateAliases.Add(alias);

        Raise(new IndustryAliasAddedEvent(Id, alias));
    }

    public void RemoveAlias(CorporateAliases alias)
    {
        ArgumentNullException.ThrowIfNull(alias);

        if (!_corporateAliases.Remove(alias))
            return;

        Raise(new IndustryAliasRemovedEvent(Id, alias));
    }
}

// Domain Events
public sealed record IndustryRenamedEvent(IndustryId IndustryId, IndustryName NewName) : DomainEvent;
public sealed record IndustryActivatedEvent(IndustryId IndustryId) : DomainEvent;
public sealed record IndustryDeactivatedEvent(IndustryId IndustryId) : DomainEvent;
public sealed record IndustryDescriptionChangedEvent(IndustryId IndustryId) : DomainEvent;
public sealed record IndustryAliasAddedEvent(IndustryId IndustryId, CorporateAliases Alias) : DomainEvent;
public sealed record IndustryAliasRemovedEvent(IndustryId IndustryId, CorporateAliases Alias) : DomainEvent;
