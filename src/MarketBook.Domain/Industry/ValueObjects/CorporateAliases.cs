// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Namespace : MarketBook.Domain.Industry.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Industry.ValueObjects;

/// <summary>
/// EN: Represents a normalized, case-insensitive collection of corporate aliases.
/// FA: مجموعه نرمال‌شده و غیرحساس به بزرگی حروف از نام‌های جایگزین شرکت.
/// </summary>
public sealed class CorporateAliases : IEquatable<CorporateAliases>
{
    private readonly HashSet<string> _aliases =
        new(StringComparer.OrdinalIgnoreCase);

    public CorporateAliases(IEnumerable<string>? aliases = null)
    {
        if (aliases is null)
            return;

        foreach (var alias in aliases)
            Add(alias);
    }

    public IReadOnlySet<string> Aliases => _aliases;

    public int Count => _aliases.Count;

    public bool HasAliases => _aliases.Count > 0;

    public void Add(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);
        _aliases.Add(Normalize(alias));
    }

    public bool Remove(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);
        return _aliases.Remove(Normalize(alias));
    }

    public bool Contains(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);
        return _aliases.Contains(Normalize(alias));
    }

    public CorporateAliases Merge(CorporateAliases other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return new CorporateAliases(_aliases.Concat(other._aliases));
    }

    public CorporateAliases WithAlias(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);
        return new CorporateAliases(_aliases.Append(alias));
    }

    public CorporateAliases WithoutAlias(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);
        var normalized = Normalize(alias);

        return new CorporateAliases(
            _aliases.Where(x => !StringComparer.OrdinalIgnoreCase.Equals(x, normalized)));
    }

    public string[] ToArray()
        => _aliases.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();

    public override string ToString()
        => string.Join(", ", ToArray());

    public bool Equals(CorporateAliases? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null || Count != other.Count)
            return false;

        return _aliases.SetEquals(other._aliases);
    }

    public override bool Equals(object? obj)
        => Equals(obj as CorporateAliases);

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var alias in _aliases.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            hash.Add(alias, StringComparer.OrdinalIgnoreCase);

        return hash.ToHashCode();
    }

    public static bool operator ==(CorporateAliases? left, CorporateAliases? right)
        => Equals(left, right);

    public static bool operator !=(CorporateAliases? left, CorporateAliases? right)
        => !Equals(left, right);

    public static CorporateAliases Empty { get; } = new();

    private static string Normalize(string value)
        => value.Trim();
}
