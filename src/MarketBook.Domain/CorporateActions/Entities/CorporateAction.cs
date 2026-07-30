// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.CorporateActions.Entities
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.CorporateActions.Enums;
using MarketBook.Domain.CorporateActions.ValueObjects;

namespace MarketBook.Domain.CorporateActions.Entities;

/// <summary>
/// EN: Base class for all corporate actions.
/// FA: کلاس پایه تمام رویدادهای شرکتی.
/// </summary>
public abstract class CorporateAction : Entity<CorporateActionId>
{
    private readonly List<IDomainEvent> _events = [];

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CorporateAction"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="CorporateAction"/> را ایجاد می‌کند.
    /// </summary>
    protected CorporateAction(
        CorporateActionId id,
        CorporateActionType type,
        DateOnly effectiveDate,
        string? description)
        : base(id)
    {
        Type = type;
        EffectiveDate = effectiveDate;
        Description = description?.Trim();
        CreatedOn = DateTimeOffset.UtcNow;
        IsApplied = false;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    protected CorporateAction()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets the corporate action type.
    /// FA: نوع رویداد شرکتی را دریافت می‌کند.
    /// </summary>
    public CorporateActionType Type { get; }

    /// <summary>
    /// EN: Gets the effective date.
    /// FA: تاریخ اعمال را دریافت می‌کند.
    /// </summary>
    public DateOnly EffectiveDate { get; }

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
    /// EN: Gets a value indicating whether the action has been applied.
    /// FA: مشخص می‌کند آیا رویداد اعمال شده است یا خیر.
    /// </summary>
    public bool IsApplied { get; private set; }

    /// <summary>
    /// EN: Gets the domain events.
    /// FA: رویدادهای دامنه را دریافت می‌کند.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    /// <summary>
    /// EN: Applies the corporate action to market data.
    /// FA: رویداد شرکتی را روی داده‌های بازار اعمال می‌کند.
    /// </summary>
    public abstract Price Apply(Price price);

    /// <summary>
    /// EN: Marks the corporate action as applied.
    /// FA: رویداد شرکتی را به عنوان اعمال‌شده علامت‌گذاری می‌کند.
    /// </summary>
    protected void MarkAsApplied()
    {
        if (IsApplied)
            return;

        IsApplied = true;
        base.AddDomainEvent(new CorporateActionAppliedEvent(Id));
    }

    /// <summary>
    /// EN: Returns a string representation of the corporate action.
    /// FA: نمایش رشته‌ای از رویداد شرکتی را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => $"{Type} on {EffectiveDate}";
}

/// <summary>
/// EN: Domain event raised when a corporate action is applied.
/// FA: رویداد دامنه زمانی که یک رویداد شرکتی اعمال می‌شود.
/// </summary>
public sealed record CorporateActionAppliedEvent(CorporateActionId CorporateActionId) : DomainEvent;