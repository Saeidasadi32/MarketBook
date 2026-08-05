// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Industry.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using System.Text;

namespace MarketBook.Domain.Industry.ValueObjects;

/// <summary>
/// EN: Represents a normalized collection of alternative company names.
/// The collection is case-insensitive and does not allow duplicate aliases.
///
/// FA: مجموعه‌ای نرمال‌شده از نام‌های جایگزین شرکت را نمایش می‌دهد.
/// این مجموعه نسبت به بزرگی و کوچکی حروف حساس نیست و نام تکراری را نمی‌پذیرد.
/// </summary>
public sealed record CorporateAliases
{
    private readonly HashSet<string> _aliases =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CorporateAliases"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="CorporateAliases"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="aliases">
    /// EN: Initial aliases.
    /// FA: نام‌های اولیه.
    /// </param>
    public CorporateAliases(IEnumerable<string>? aliases = null)
    {
        if (aliases is null)
            return;

        foreach (string? alias in aliases
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            _aliases.Add(Normalize(alias));
        }
    }

    /// <summary>
    /// EN: Gets all aliases.
    /// FA: همه نام‌های جایگزین را دریافت می‌کند.
    /// </summary>
    public IReadOnlySet<string> Aliases => _aliases;

    /// <summary>
    /// EN: Gets the number of aliases.
    /// FA: تعداد نام‌های جایگزین را دریافت می‌کند.
    /// </summary>
    public int Count => _aliases.Count;

    /// <summary>
    /// EN: Gets a value indicating whether any aliases exist.
    /// FA: مشخص می‌کند که آیا نام جایگزینی وجود دارد یا خیر.
    /// </summary>
    public bool HasAliases => _aliases.Count > 0;

    /// <summary>
    /// EN: Adds a new alias.
    /// FA: یک نام جایگزین جدید اضافه می‌کند.
    /// </summary>
    /// <param name="alias">
    /// EN: Alias to add.
    /// FA: نام جایگزین.
    /// </param>
    public void Add(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);

        _aliases.Add(Normalize(alias));
    }

    /// <summary>
    /// EN: Removes an alias.
    /// FA: یک نام جایگزین را حذف می‌کند.
    /// </summary>
    public bool Remove(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);

        return _aliases.Remove(Normalize(alias));
    }

    /// <summary>
    /// EN: Determines whether the specified alias exists.
    /// FA: بررسی می‌کند که آیا نام جایگزین وجود دارد یا خیر.
    /// </summary>
    public bool Contains(string alias)
    {
        Guard.AgainstNullOrWhiteSpace(alias);

        return _aliases.Contains(Normalize(alias));
    }

    /// <summary>
    /// EN: Creates a merged alias collection.
    /// FA: یک مجموعه ادغام‌شده از نام‌های جایگزین ایجاد می‌کند.
    /// </summary>
    public CorporateAliases Merge(CorporateAliases other)
    {
        Guard.AgainstNull(other);

        return new CorporateAliases(_aliases.Concat(other._aliases));
    }

    /// <summary>
    /// EN: Returns a new instance with an additional alias.
    /// FA: نمونه جدیدی با یک نام جایگزین اضافه‌شده برمی‌گرداند.
    /// </summary>
    public CorporateAliases WithAlias(string alias)
    {
        Guard.AgainstNullOrWhiteSpace(alias);

        return new(_aliases.Append(alias));
    }

    /// <summary>
    /// EN: Returns a new instance without the specified alias.
    /// FA: نمونه جدیدی بدون نام جایگزین مشخص‌شده برمی‌گرداند.
    /// </summary>
    public CorporateAliases WithoutAlias(string alias)
    {
        Guard.AgainstNullOrWhiteSpace(alias);

        string normalized = Normalize(alias);

        return new(
            _aliases.Where(x =>
                !string.Equals(
                    x,
                    normalized,
                    StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// EN: Returns a comma-separated string of aliases ordered alphabetically.
    /// FA: نام‌های جایگزین را به‌صورت رشته‌ای مرتب‌شده و جداشده با ویرگول برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => string.Join(", ", _aliases.OrderBy(x => x));

    /// <summary>
    /// EN: Converts the aliases to an array.
    /// FA: نام‌های جایگزین را به آرایه تبدیل می‌کند.
    /// </summary>
    public string[] ToArray()
        => _aliases.OrderBy(x => x).ToArray();

    /// <summary>
    /// EN: Gets an empty alias collection.
    /// FA: یک مجموعه خالی از نام‌های جایگزین را برمی‌گرداند.
    /// </summary>
    public static CorporateAliases Empty { get; } = new();

    private static string Normalize(string value)
    {
        string normalized = value
            .Trim()
            .Normalize(NormalizationForm.FormKC);

        Guard.AgainstNullOrWhiteSpace(normalized);

        return normalized;
    }
}
