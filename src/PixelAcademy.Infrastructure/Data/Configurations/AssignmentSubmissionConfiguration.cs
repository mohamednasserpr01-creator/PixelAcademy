using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class AssignmentSubmissionConfiguration : IEntityTypeConfiguration<AssignmentSubmission>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => new { s.StudentId, s.AssignmentId });

        builder.HasOne(s => s.Assignment).WithMany(a => a.Submissions).HasForeignKey(s => s.AssignmentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(s => s.Student).WithMany(u => u.AssignmentSubmissions).HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.Cascade);
    }
}
