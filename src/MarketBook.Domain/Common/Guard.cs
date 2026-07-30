using System.Runtime.CompilerServices;

namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Guard helper methods.
/// FA: متدهای کمکی اعتبارسنجی.
/// </summary>
public static class Guard
{
    public static void AgainstNull(
        object? value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        ArgumentNullException.ThrowIfNull(value, name);
    }

    public static void AgainstNullOrWhiteSpace(
        string? value,
        [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            value,
            name);
    }
}