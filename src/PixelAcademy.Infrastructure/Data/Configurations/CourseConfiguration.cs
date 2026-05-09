using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(4000);
        builder.Property(c => c.ShortDescription).HasMaxLength(500);
        builder.Property(c => c.ThumbnailUrl).HasMaxLength(500);
        builder.Property(c => c.TrailerUrl).HasMaxLength(500);
        builder.Property(c => c.Level).HasConversion<string>().HasMaxLength(50);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(c => c.Price).HasPrecision(18, 2);
        builder.Property(c => c.Category).HasMaxLength(100);
        builder.Property(c => c.Tags).HasMaxLength(500);
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasOne(c => c.Instructor).WithMany(u => u.OwnedCourses).HasForeignKey(c => c.InstructorId);
        builder.HasMany(c => c.Lectures).WithOne(l => l.Course).HasForeignKey(l => l.CourseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c => c.Enrollments).WithOne(e => e.Course).HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c => c.MediaAssets).WithOne(m => m.Course).HasForeignKey(m => m.CourseId).OnDelete(DeleteBehavior.SetNull);
    }
}
