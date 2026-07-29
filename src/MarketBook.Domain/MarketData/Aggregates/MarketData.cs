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
public sealed class MarketData : AggregateRoot
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="MarketData"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="MarketData"/> را ایجاد می‌کند.
    /// </summary>
    public MarketData(
        MarketDataId id,
        ListingId listingId,
        TradingDate tradingDate,
        DailySnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        Id = id;
        ListingId = listingId;
        TradingDate = tradingDate;
        Snapshot = snapshot;
    }

    /// <summary>
    /// EN: Gets market data identifier.
    /// FA: شناسه داده بازار را دریافت می‌کند.
    /// </summary>
    public MarketDataId Id { get; }

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
    /// EN: Stores corporate actions affecting this trading day.
    /// FA: رویدادهای شرکتی مؤثر بر این روز معاملاتی را نگهداری می‌کند.
    /// </summary>
    private readonly List<CorporateAction> _corporateActions = [];

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
    /// <param name="corporateAction">
    /// EN: Corporate action.
    /// FA: رویداد شرکتی.
    /// </param>
    public void AddCorporateAction(CorporateAction corporateAction)
    {
        ArgumentNullException.ThrowIfNull(corporateAction);

        if (_corporateActions.Any(x => x.Id == corporateAction.Id))
            return;

        _corporateActions.Add(corporateAction);
    }

    /// <summary>
    /// EN: Removes a corporate action.
    /// FA: یک رویداد شرکتی را حذف می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Corporate action identifier.
    /// FA: شناسه رویداد.
    /// </param>
    public void RemoveCorporateAction(CorporateActionId id)
    {
        _corporateActions.RemoveAll(x => x.Id == id);
    }

    /// <summary>
    /// EN: Replaces the current snapshot.
    /// FA: Snapshot فعلی را جایگزین می‌کند.
    /// </summary>
    public void UpdateSnapshot(DailySnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        Snapshot = snapshot;
    }
}