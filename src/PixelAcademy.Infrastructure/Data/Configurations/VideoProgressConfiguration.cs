using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class VideoProgressConfiguration : IEntityTypeConfiguration<VideoProgress>
{
    public void Configure(EntityTypeBuilder<VideoProgress> builder)
    {
        builder.HasKey(vp => vp.Id);
        builder.HasIndex(vp => new { vp.StudentId, vp.LectureId }).IsUnique();

        builder.HasOne(vp => vp.Student).WithMany(u => u.VideoProgresses).HasForeignKey(vp => vp.StudentId);
        builder.HasOne(vp => vp.Lecture).WithMany(l => l.VideoProgresses).HasForeignKey(vp => vp.LectureId);
    }
}
