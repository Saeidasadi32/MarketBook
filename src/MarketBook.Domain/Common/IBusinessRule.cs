namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Represents a business rule.
/// FA: یک قانون کسب‌وکار را نمایش می‌دهد.
/// </summary>
public interface IBusinessRule
{
    /// <summary>
    /// EN: Determines whether rule is broken.
    /// FA: مشخص می‌کند قانون نقض شده است یا خیر.
    /// </summary>
    bool IsBroken();

    /// <summary>
    /// EN: Gets rule message.
    /// FA: پیام قانون.
    /// </summary>
    string Message { get; }
}