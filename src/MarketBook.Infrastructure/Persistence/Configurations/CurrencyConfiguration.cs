// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace MarketBook.Infrastructure.Persistence.Configurations;
/// <summary>EN: Configures Currency persistence. FA: نگاشت ذخیره‌سازی Currency را پیکربندی می‌کند.</summary>
public sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
 /// <summary>EN: Configures the entity mapping. FA: نگاشت موجودیت را پیکربندی می‌کند.</summary>
 public void Configure(EntityTypeBuilder<Currency> builder){ArgumentNullException.ThrowIfNull(builder);builder.ToTable("Currencies");builder.HasKey(x=>x.Id);ValueConverter<CurrencyId,string> id=new(x=>x.Value.ToString(),x=>CurrencyId.Parse(x));builder.Property(x=>x.Id).HasConversion(id).HasMaxLength(26).IsUnicode(false).IsRequired();ValueConverter<CurrencyCode,string> code=new(x=>x.Value,x=>new CurrencyCode(x));builder.Property(x=>x.Code).HasConversion(code).HasMaxLength(3).IsUnicode(false).IsRequired();builder.HasIndex(x=>x.Code).IsUnique();builder.Property(x=>x.Name).HasMaxLength(200).IsUnicode().IsRequired();builder.Property(x=>x.DecimalPlaces).HasColumnType("tinyint").IsRequired();builder.Property(x=>x.CreatedOn).IsRequired();builder.Property(x=>x.IsActive).IsRequired();}
}
