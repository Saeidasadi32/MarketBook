// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.CorporateActions.Entities
// -----------------------------------------------------------------------------

using MarketBook.Domain.CorporateActions.Enums;
using MarketBook.Domain.CorporateActions.ValueObjects;

namespace MarketBook.Domain.CorporateActions.Entities;

/// <summary>
/// EN: Base class for all corporate actions.
/// FA: کلاس پایه تمام رویدادهای شرکتی.
/// </summary>
public abstract class CorporateAction
{
    protected CorporateAction(
        CorporateActionId id,
        CorporateActionType type,
        DateOnly effectiveDate,
        string? description)
    {
        Id = id;
        Type = type;
        EffectiveDate = effectiveDate;
        Description = description?.Trim();
    }

    /// <summary>
    /// EN: Corporate action identifier.
    /// FA: شناسه رویداد.
    /// </summary>
    public CorporateActionId Id { get; }

    /// <summary>
    /// EN: Action type.
    /// FA: نوع رویداد.
    /// </summary>
    public CorporateActionType Type { get; }

    /// <summary>
    /// EN: Effective date.
    /// FA: تاریخ اعمال.
    /// </summary>
    public DateOnly EffectiveDate { get; }

    /// <summary>
    /// EN: Description.
    /// FA: توضیحات.
    /// </summary>
    public string? Description { get; }
}