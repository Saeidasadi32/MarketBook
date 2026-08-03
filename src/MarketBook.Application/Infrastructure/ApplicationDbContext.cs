// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Application.Infrastructure;

/// <summary>
/// EN: Represents the primary Entity Framework Core database context.
/// FA: کانتکست اصلی Entity Framework Core سیستم را نمایش می‌دهد.
/// </summary>
public sealed class ApplicationDbContext :
    DbContext,
    IApplicationDbContext
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="ApplicationDbContext"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="options">
    /// EN: DbContext options.
    /// FA: تنظیمات DbContext.
    /// </param>
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// EN: Configures the EF Core model.
    /// FA: مدل Entity Framework Core را پیکربندی می‌کند.
    /// </summary>
    /// <param name="modelBuilder">
    /// EN: Model builder.
    /// FA: سازنده مدل.
    /// </param>
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}
