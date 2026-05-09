using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class LectureConfiguration : IEntityTypeConfiguration<Lecture>
{
    public void Configure(EntityTypeBuilder<Lecture> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Title).HasMaxLength(200).IsRequired();
        builder.Property(l => l.Description).HasMaxLength(2000);
        builder.Property(l => l.VideoUrl).HasMaxLength(500);
        builder.HasIndex(l => new { l.CourseId, l.OrderIndex });
        builder.HasQueryFilter(l => !l.IsDeleted);

        builder.HasOne(l => l.Course).WithMany(c => c.Lectures).HasForeignKey(l => l.CourseId);
        builder.HasMany(l => l.MediaAssets).WithOne(m => m.Lecture).HasForeignKey(m => m.LectureId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(l => l.VideoProgresses).WithOne(vp => vp.Lecture).HasForeignKey(vp => vp.LectureId).OnDelete(DeleteBehavior.Cascade);
    }
}
