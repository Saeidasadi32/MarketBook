// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Common;
using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Financial.ValueObjects;
using MarketBook.Domain.Investor.ValueObjects;
using MarketBook.Domain.Portfolio.Entities;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;

namespace MarketBook.Domain.Portfolio.Aggregates;

/// <summary>
/// EN: Represents an investment portfolio owned by an investor.
/// FA: یک پرتفوی سرمایه‌گذاری متعلق به یک سرمایه‌گذار را نمایش می‌دهد.
/// </summary>
public sealed class Portfolio : AggregateRoot<PortfolioId>
{
    private readonly List<Position> _positions = [];
    private readonly List<PortfolioEvent> _events = [];

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Portfolio"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Portfolio"/> را ایجاد می‌کند.
    /// </summary>
    public Portfolio(
        PortfolioId id,
        InvestorId investorId,
        PortfolioName name)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(investorId);
        ArgumentNullException.ThrowIfNull(name);

        InvestorId = investorId;
        Name = name;
        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Portfolio()
    {
        InvestorId = default!;
        Name = default!;
    }

    /// <summary>
    /// EN: Gets investor identifier.
    /// FA: شناسه سرمایه‌گذار را دریافت می‌کند.
    /// </summary>
    public InvestorId InvestorId { get; private set; }

    /// <summary>
    /// EN: Gets portfolio name.
    /// FA: نام پرتفوی را دریافت می‌کند.
    /// </summary>
    public PortfolioName Name { get; private set; }

    /// <summary>
    /// EN: Gets the optional reporting/base currency for portfolio-level translation.
    /// FA: ارز پایه/گزارش‌دهی اختیاری پرتفوی برای ترجمه مقادیر سطح پرتفوی را دریافت می‌کند.
    /// </summary>
    public CurrencyId? BaseCurrencyId { get; private set; }

    /// <summary>
    /// EN: Gets creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// EN: Gets whether portfolio is active.
    /// FA: فعال بودن پرتفوی را دریافت می‌کند.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Gets all open positions.
    /// FA: موقعیت‌های باز پرتفوی را دریافت می‌کند.
    /// </summary>
    public IReadOnlyCollection<Position> Positions
        => _positions.AsReadOnly();

    /// <summary>
    /// EN: Gets portfolio history.
    /// FA: تاریخچه رویدادهای پرتفوی را دریافت می‌کند.
    /// </summary>
    public IReadOnlyCollection<PortfolioEvent> Events
        => _events.AsReadOnly();

    /// <summary>
    /// EN: Creates a portfolio with a generated identifier.
    /// FA: یک پرتفوی با شناسه تولیدشده ایجاد می‌کند.
    /// </summary>
    public static Portfolio Create(
        InvestorId investorId,
        PortfolioName name)
    {
        ArgumentNullException.ThrowIfNull(investorId);
        ArgumentNullException.ThrowIfNull(name);

        return new Portfolio(PortfolioId.New(), investorId, name);
    }

    /// <summary>
    /// EN: Sets the portfolio reporting/base currency.
    /// FA: ارز پایه/گزارش‌دهی پرتفوی را تنظیم می‌کند.
    /// </summary>
    /// <param name="currencyId">EN: Active currency identifier. FA: شناسه ارز فعال.</param>
    public void SetBaseCurrency(CurrencyId currencyId)
    {
        ArgumentNullException.ThrowIfNull(currencyId);

        if (BaseCurrencyId == currencyId)
        {
            return;
        }

        BaseCurrencyId = currencyId;
    }

    /// <summary>
    /// EN: Renames the portfolio.
    /// FA: نام پرتفوی را تغییر می‌دهد.
    /// </summary>
    public void Rename(PortfolioName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (Name == name)
            return;

        Name = name;
        Raise(new PortfolioRenamedEvent(Id, name));
    }

    /// <summary>
    /// EN: Activates the portfolio.
    /// FA: پرتفوی را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        Raise(new PortfolioActivatedEvent(Id));
    }

    /// <summary>
    /// EN: Deactivates the portfolio.
    /// FA: پرتفوی را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        Raise(new PortfolioDeactivatedEvent(Id));
    }

    /// <summary>
    /// EN: Registers a portfolio event (Buy/Sell).
    /// FA: یک رویداد پرتفوی (خرید/فروش) را ثبت می‌کند.
    /// </summary>
    public void RegisterEvent(PortfolioEvent portfolioEvent)
    {
        ArgumentNullException.ThrowIfNull(portfolioEvent);

        if (!IsActive)
            throw new DomainException(
                new Error(
                    "Portfolio.Inactive",
                    "Cannot register events on an inactive portfolio."));

        _events.Add(portfolioEvent);

        Position? position = _positions.FirstOrDefault(
            item => item.ListingId == portfolioEvent.ListingId);

        if (position is null)
        {
            if (portfolioEvent.Type != PortfolioEventType.Buy)
                throw new DomainException(
                    new Error(
                        "Portfolio.FirstEventMustBeBuy",
                        "The first event of a position must be Buy."));

            position = new Position(
                PositionId.New(),
                portfolioEvent.ListingId,
                PositionSide.Long,
                Quantity.Zero,
                Money.Zero);

            _positions.Add(position);
        }

        position.Apply(portfolioEvent);

        if (position.Quantity.IsZero)
        {
            _positions.Remove(position);
        }

        Raise(new PortfolioEventRegisteredEvent(Id, portfolioEvent));
    }

    /// <summary>
    /// EN: Calculates the total value of the portfolio.
    /// FA: ارزش کل پرتفوی را محاسبه می‌کند.
    /// </summary>
    public Money CalculateTotalValue()
    {
        return _positions.Aggregate(
            Money.Zero,
            (current, position) => current + position.MarketValue);
    }

    /// <summary>
    /// EN: Calculates the total invested amount.
    /// FA: کل سرمایه‌گذاری شده را محاسبه می‌کند.
    /// </summary>
    public Money CalculateTotalInvested()
    {
        return _positions.Aggregate(
            Money.Zero,
            (current, position) => current + position.TotalCost);
    }
}

/// <summary>
/// EN: Domain event raised when a portfolio is renamed.
/// FA: رویداد دامنه زمانی که نام پرتفوی تغییر می‌کند.
/// </summary>
public sealed record PortfolioRenamedEvent(
    PortfolioId PortfolioId,
    PortfolioName NewName) : DomainEvent;

/// <summary>
/// EN: Domain event raised when a portfolio is activated.
/// FA: رویداد دامنه زمانی که پرتفوی فعال می‌شود.
/// </summary>
public sealed record PortfolioActivatedEvent(PortfolioId PortfolioId) : DomainEvent;

/// <summary>
/// EN: Domain event raised when a portfolio is deactivated.
/// FA: رویداد دامنه زمانی که پرتفوی غیرفعال می‌شود.
/// </summary>
public sealed record PortfolioDeactivatedEvent(PortfolioId PortfolioId) : DomainEvent;

/// <summary>
/// EN: Domain event raised when an event is registered on a portfolio.
/// FA: رویداد دامنه زمانی که یک رویداد روی پرتفوی ثبت می‌شود.
/// </summary>
public sealed record PortfolioEventRegisteredEvent(
    PortfolioId PortfolioId,
    PortfolioEvent PortfolioEvent) : DomainEvent;
