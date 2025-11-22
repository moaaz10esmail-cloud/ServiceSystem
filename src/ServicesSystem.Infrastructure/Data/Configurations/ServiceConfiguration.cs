using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicesSystem.Domain.Entities;

namespace ServicesSystem.Infrastructure.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.NameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.DescriptionAr)
            .HasMaxLength(1000);

        builder.Property(s => s.BasePrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(s => s.Category)
            .WithMany(c => c.Services)
            .HasForeignKey(s => s.CategoryId);

        builder.HasMany(s => s.Requests)
            .WithOne(r => r.Service)
            .HasForeignKey(r => r.ServiceId);
    }
}
