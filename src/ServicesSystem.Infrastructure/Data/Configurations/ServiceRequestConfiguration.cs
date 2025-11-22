using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicesSystem.Domain.Entities;

namespace ServicesSystem.Infrastructure.Data.Configurations;

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.ToTable("ServiceRequests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.OwnsOne(r => r.CustomerLocation, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("State").HasMaxLength(100);
            address.Property(a => a.PostalCode).HasColumnName("PostalCode").HasMaxLength(20);
            address.Property(a => a.Country).HasColumnName("Country").HasMaxLength(100);
            address.Property(a => a.AdditionalInfo).HasColumnName("AdditionalInfo").HasMaxLength(500);
        });

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(r => r.Customer)
            .WithMany(u => u.CustomerRequests)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Technician)
            .WithMany(u => u.TechnicianRequests)
            .HasForeignKey(r => r.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Service)
            .WithMany(s => s.Requests)
            .HasForeignKey(r => r.ServiceId);

        builder.HasOne(r => r.Payment)
            .WithOne(p => p.Request)
            .HasForeignKey<Payment>(p => p.RequestId);

        builder.HasOne(r => r.Review)
            .WithOne(rv => rv.Request)
            .HasForeignKey<Review>(rv => rv.RequestId);

        builder.HasMany(r => r.Trackings)
            .WithOne(t => t.Request)
            .HasForeignKey(t => t.RequestId);
    }
}
