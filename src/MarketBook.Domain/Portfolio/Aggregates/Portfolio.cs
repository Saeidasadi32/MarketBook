using MarketBook.Domain.Common;
using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.Financial.ValueObjects;
using MarketBook.Domain.Investor.ValueObjects;
using MarketBook.Domain.Portfolio.Entities;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;
using System.Transactions;

namespace MarketBook.Domain.Portfolio.Aggregates;

/// <summary>
/// EN: Represents an investment portfolio.
/// FA: یک پرتفوی سرمایه‌گذاری را نمایش می‌دهد.
/// </summary>
public sealed class Portfolio : AggregateRoot
{
    public Portfolio(
        PortfolioId id,
        InvestorId investorId,
        PortfolioName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Id = id;
        InvestorId = investorId;
        CreatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// EN: Gets portfolio identifier.
    /// FA: شناسه پرتفوی را دریافت می‌کند.
    /// </summary>
    public PortfolioId Id { get; }

    /// <summary>
    /// EN: Gets investor identifier.
    /// FA: شناسه سرمایه‌گذار را دریافت می‌کند.
    /// </summary>
    public InvestorId InvestorId { get; }

    /// <summary>
    /// EN: Gets portfolio name.
    /// FA: نام پرتفوی را دریافت می‌کند.
    /// </summary>
    public PortfolioName Name { get; private set; }

    /// <summary>
    /// EN: Gets creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Renames portfolio.
    /// FA: نام پرتفوی را تغییر می‌دهد.
    /// </summary>
    public void Rename(PortfolioName name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    /// <summary>
    /// EN: Gets whether portfolio is active.
    /// FA: فعال بودن پرتفوی.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    /// <summary>
    /// EN: Portfolio positions.
    /// FA: موقعیت‌های پرتفوی.
    /// </summary>
    private readonly List<Position> _positions = [];

    /// <summary>
    /// EN: Portfolio events.
    /// FA: رویدادهای پرتفوی.
    /// </summary>
    private readonly List<PortfolioEvent> _events = [];

    /// <summary>
    /// EN: Gets all open positions.
    /// FA: موقعیت‌های باز پرتفوی.
    /// </summary>
    public IReadOnlyCollection<Position> Positions
        => _positions.AsReadOnly();

    /// <summary>
    /// EN: Gets portfolio history.
    /// FA: تاریخچه رویدادهای پرتفوی.
    /// </summary>
    public IReadOnlyCollection<PortfolioEvent> Events
        => _events.AsReadOnly();

    /// <summary>
    /// EN: Registers a portfolio event.
    /// FA: یک رویداد پرتفوی را ثبت می‌کند.
    /// </summary>
    public void RegisterEvent(PortfolioEvent portfolioEvent)
    {
        ArgumentNullException.ThrowIfNull(portfolioEvent);

        _events.Add(portfolioEvent);

        var position = _positions.FirstOrDefault(
            x => x.ListingId == portfolioEvent.ListingId);

        if (position is null)
        {
            if (portfolioEvent.Type != PortfolioEventType.Buy)
                throw new InvalidOperationException(
                    "The first event of a position must be Buy.");

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
    }
}