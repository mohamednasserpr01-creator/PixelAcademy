using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class ActivationCodeConfiguration : IEntityTypeConfiguration<ActivationCode>
{
    public void Configure(EntityTypeBuilder<ActivationCode> builder)
    {
        builder.HasKey(ac => ac.Id);
        builder.Property(ac => ac.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(ac => ac.Code).IsUnique();
        builder.Property(ac => ac.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(ac => ac.Value).HasPrecision(18, 2);
        builder.Property(ac => ac.MaxRedemptions).HasDefaultValue(1);

        builder.HasOne(ac => ac.Course).WithMany().HasForeignKey(ac => ac.CourseId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(ac => ac.Lecture).WithMany().HasForeignKey(ac => ac.LectureId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(ac => ac.GeneratedBy).WithMany(u => u.GeneratedCodes).HasForeignKey(ac => ac.GeneratedById).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(ac => ac.LastRedeemedBy).WithMany().HasForeignKey(ac => ac.LastRedeemedById).OnDelete(DeleteBehavior.SetNull);
    }
}
