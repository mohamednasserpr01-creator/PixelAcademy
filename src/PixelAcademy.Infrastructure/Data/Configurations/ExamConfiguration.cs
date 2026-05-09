using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.CourseId);
        builder.HasIndex(e => e.LectureId);
        builder.HasIndex(e => e.CreatedById);

        builder.HasOne(e => e.Course).WithMany().HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.Lecture).WithMany().HasForeignKey(e => e.LectureId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.Creator).WithMany(u => u.CreatedExams).HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
