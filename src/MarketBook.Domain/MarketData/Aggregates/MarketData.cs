// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.MarketData.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.CorporateActions.Entities;
using MarketBook.Domain.CorporateActions.ValueObjects;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.Snapshots;
using MarketBook.Domain.MarketData.ValueObjects;

namespace MarketBook.Domain.MarketData.Aggregates;

/// <summary>
/// EN: Represents all market information for one listing on one trading day.
/// FA: تمام اطلاعات بازار یک نماد در یک روز معاملاتی را نمایش می‌دهد.
/// </summary>
public sealed class MarketData : AggregateRoot<MarketDataId>
{
    private readonly List<CorporateAction> _corporateActions = [];

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="MarketData"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="MarketData"/> را ایجاد می‌کند.
    /// </summary>
    public MarketData(
        MarketDataId id,
        ListingId listingId,
        TradingDate tradingDate,
        DailySnapshot snapshot)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        ListingId = listingId;
        TradingDate = tradingDate;
        Snapshot = snapshot;
        CreatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private MarketData()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets listing identifier.
    /// FA: شناسه پذیرش را دریافت می‌کند.
    /// </summary>
    public ListingId ListingId { get; }

    /// <summary>
    /// EN: Gets trading date.
    /// FA: تاریخ معاملاتی را دریافت می‌کند.
    /// </summary>
    public TradingDate TradingDate { get; }

    /// <summary>
    /// EN: Gets the daily market snapshot.
    /// FA: Snapshot روزانه بازار را دریافت می‌کند.
    /// </summary>
    public DailySnapshot Snapshot { get; private set; }

    /// <summary>
    /// EN: Gets the creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets all corporate actions.
    /// FA: تمام رویدادهای شرکتی را دریافت می‌کند.
    /// </summary>
    public IReadOnlyCollection<CorporateAction> CorporateActions
        => _corporateActions.AsReadOnly();

    /// <summary>
    /// EN: Adds a corporate action.
    /// FA: یک رویداد شرکتی را اضافه می‌کند.
    /// </summary>
    public void AddCorporateAction(CorporateAction corporateAction)
    {
        ArgumentNullException.ThrowIfNull(corporateAction);

        if (_corporateActions.Any(x => x.Id == corporateAction.Id))
            return;

        _corporateActions.Add(corporateAction);
        Raise(new CorporateActionAddedEvent(Id, corporateAction));
    }

    /// <summary>
    /// EN: Removes a corporate action.
    /// FA: یک رویداد شرکتی را حذف می‌کند.
    /// </summary>
    public void RemoveCorporateAction(CorporateActionId id)
    {
        var removed = _corporateActions.RemoveAll(x => x.Id == id);
        if (removed > 0)
        {
            Raise(new CorporateActionRemovedEvent(Id, id));
        }
    }

    /// <summary>
    /// EN: Replaces the current snapshot.
    /// FA: Snapshot فعلی را جایگزین می‌کند.
    /// </summary>
    public void UpdateSnapshot(DailySnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (Snapshot == snapshot)
            return;

        Snapshot = snapshot;
        Raise(new MarketDataSnapshotUpdatedEvent(Id, snapshot));
    }
}

/// <summary>
/// EN: Domain event raised when a corporate action is added.
/// FA: رویداد دامنه زمانی که یک رویداد شرکتی اضافه می‌شود.
/// </summary>
public sealed record CorporateActionAddedEvent(MarketDataId MarketDataId, CorporateAction CorporateAction) : DomainEvent;

/// <summary>
/// EN: Domain event raised when a corporate action is removed.
/// FA: رویداد دامنه زمانی که یک رویداد شرکتی حذف می‌شود.
/// </summary>
public sealed record CorporateActionRemovedEvent(MarketDataId MarketDataId, CorporateActionId CorporateActionId) : DomainEvent;

/// <summary>
/// EN: Domain event raised when a snapshot is updated.
/// FA: رویداد دامنه زمانی که Snapshot به‌روزرسانی می‌شود.
/// </summary>
public sealed record MarketDataSnapshotUpdatedEvent(MarketDataId MarketDataId, DailySnapshot NewSnapshot) : DomainEvent;