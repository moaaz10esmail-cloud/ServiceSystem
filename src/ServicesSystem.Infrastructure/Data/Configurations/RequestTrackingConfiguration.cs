using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicesSystem.Domain.Entities;

namespace ServicesSystem.Infrastructure.Data.Configurations;

public class RequestTrackingConfiguration : IEntityTypeConfiguration<RequestTracking>
{
    public void Configure(EntityTypeBuilder<RequestTracking> builder)
    {
        builder.ToTable("RequestTrackings");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Notes)
            .HasMaxLength(500);

        builder.OwnsOne(t => t.CurrentLocation, location =>
        {
            location.Property(l => l.Latitude).HasColumnName("CurrentLatitude");
            location.Property(l => l.Longitude).HasColumnName("CurrentLongitude");
        });

        builder.HasOne(t => t.Request)
            .WithMany(r => r.Trackings)
            .HasForeignKey(t => t.RequestId);
    }
}
