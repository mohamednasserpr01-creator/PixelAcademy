using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class ExamAttemptConfiguration : IEntityTypeConfiguration<ExamAttempt>
{
    public void Configure(EntityTypeBuilder<ExamAttempt> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => new { a.StudentId, a.ExamId });
        builder.HasIndex(a => a.StartedAt);

        builder.HasOne(a => a.Exam).WithMany(e => e.Attempts).HasForeignKey(a => a.ExamId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.Student).WithMany(u => u.ExamAttempts).HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.Cascade);
    }
}
