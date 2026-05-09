using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.Review).HasMaxLength(2000);
        builder.Property(e => e.ProgressPercent).HasPrecision(5, 2);
        builder.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

        builder.HasOne(e => e.Student).WithMany(u => u.Enrollments).HasForeignKey(e => e.StudentId);
        builder.HasOne(e => e.Course).WithMany(c => c.Enrollments).HasForeignKey(e => e.CourseId);
        builder.HasOne(e => e.ActivationCode).WithMany().HasForeignKey(e => e.ActivationCodeId).OnDelete(DeleteBehavior.SetNull);
    }
}
