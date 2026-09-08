// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Domain.Investor.Aggregates;
using MarketBook.Domain.Investor.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>EN: Configures Investor persistence. FA: نگاشت ماندگاری Investor را پیکربندی می‌کند.</summary>
public sealed class InvestorConfiguration : IEntityTypeConfiguration<Investor>
{
    /// <summary>EN: Configures entity mapping. FA: نگاشت موجودیت را پیکربندی می‌کند.</summary>
    public void Configure(EntityTypeBuilder<Investor> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Investors");
        builder.HasKey(item => item.Id);

        ValueConverter<InvestorId, string> idConverter = new(
            id => id.Value.ToString(),
            value => InvestorId.Parse(value));

        builder.Property(item => item.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(item => item.FullName)
            .HasMaxLength(200)
            .IsUnicode()
            .IsRequired();

        builder.Property(item => item.CreatedOn)
            .IsRequired();

        builder.Property(item => item.IsActive)
            .IsRequired();

        builder.HasIndex(item => item.FullName)
            .HasDatabaseName("IX_Investors_FullName");
    }
}
