// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using NUlid;
using System.Diagnostics;

namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Represents the base type for all strongly typed entity identifiers.
/// Every identifier in the domain is based on a ULID value.
///
/// FA: کلاس پایه برای تمام شناسه‌های Strongly Typed دامنه.
/// تمام شناسه‌های دامنه بر پایه ULID هستند.
/// </summary>
[DebuggerDisplay("{Value}")]
public abstract record EntityId
{
    /// <summary>
    /// EN: Initializes a new identifier.
    ///
    /// FA: یک شناسه جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="value">
    /// EN: Underlying ULID value.
    ///
    /// FA: مقدار ULID.
    /// </param>
    protected EntityId(Ulid value)
    {
        if (value == Ulid.Empty)
        {
            throw new ArgumentException(
                "Entity identifier cannot be empty.",
                nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// EN: Gets identifier value.
    ///
    /// FA: مقدار شناسه.
    /// </summary>
    public Ulid Value { get; }

    /// <summary>
    /// EN: Converts this identifier to ULID.
    ///
    /// FA: این شناسه را به ULID تبدیل می‌کند.
    /// </summary>
    public Ulid ToUlid()
        => Value;

    /// <summary>
    /// EN: Returns string representation.
    ///
    /// FA: نمایش متنی شناسه.
    /// </summary>
    public override string ToString()
        => Value.ToString();

    /// <summary>
    /// EN: Implicit conversion to ULID.
    ///
    /// FA: تبدیل ضمنی به ULID.
    /// </summary>
    public static implicit operator Ulid(EntityId id)
    {
        Guard.AgainstNull(id);
        return id.Value;
    }
}
