// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Context
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Context;

/// <summary>
/// EN: Represents the primary Entity Framework Core database context.
/// FA: کانتکست اصلی Entity Framework Core سیستم را نمایش می‌دهد.
/// </summary>
public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// EN: Initializes the database context.
    /// FA: کانتکست پایگاه داده را ایجاد می‌کند.
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// EN: Configures the EF Core model.
    /// FA: مدل EF Core را پیکربندی می‌کند.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}
