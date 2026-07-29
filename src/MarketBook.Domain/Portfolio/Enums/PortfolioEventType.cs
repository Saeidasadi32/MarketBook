// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.Enums
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Portfolio.Enums;

/// <summary>
/// EN: Represents portfolio transaction types.
/// FA: انواع عملیات روی پرتفوی.
/// </summary>
public enum PortfolioEventType
{
    Buy = 1,

    Sell = 2,

    Dividend = 3,

    CapitalIncrease = 4,

    BonusShares = 5,

    Split = 6,

    ReverseSplit = 7,

    Deposit = 8,

    Withdraw = 9,

    Fee = 10,

    Tax = 11,

    Interest = 12,

    TransferIn = 13,

    TransferOut = 14,

    OptionAssignment = 15,

    OptionExercise = 16,

    FuturesSettlement = 17,

    SymbolChange = 18,

    Custom = 1000
}