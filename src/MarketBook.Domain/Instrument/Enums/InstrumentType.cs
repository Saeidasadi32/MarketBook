// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Instrument.Enums
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Instrument.Enums;

/// <summary>
/// EN: Represents the type of a tradable instrument.
/// FA: نوع ابزار مالی قابل معامله را مشخص می‌کند.
/// </summary>
public enum InstrumentType
{
    Stock = 1,
    Option = 2,
    Future = 3,
    ETF = 4,
    MutualFund = 5,
    Bond = 6,
    Commodity = 7,
    Gold = 8,
    Currency = 9,
    Cryptocurrency = 10,
    Index = 11,
    Custom = 1000
}