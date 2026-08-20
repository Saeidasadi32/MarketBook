namespace MarketBook.Domain.Venue.Enums;

/// <summary>
/// EN: Represents the type of a trading venue.
/// FA: نوع بستر اجرای معاملات را مشخص می‌کند.
/// </summary>
public enum VenueType
{
    Exchange = 1,
    CryptoExchange = 2,
    AlternativeTradingSystem = 3,
    DarkPool = 4,
    OverTheCounter = 5,
    DecentralizedExchange = 6,
    Broker = 7,
    Internal = 8,
    Other = 1000
}
