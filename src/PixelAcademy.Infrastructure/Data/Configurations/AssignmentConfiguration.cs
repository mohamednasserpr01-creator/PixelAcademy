using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.CourseId);
        builder.HasIndex(a => a.LectureId);
        builder.HasIndex(a => a.CreatedById);

        builder.HasOne(a => a.Course).WithMany().HasForeignKey(a => a.CourseId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(a => a.Lecture).WithMany().HasForeignKey(a => a.LectureId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(a => a.Creator).WithMany(u => u.CreatedAssignments).HasForeignKey(a => a.CreatedById).OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
