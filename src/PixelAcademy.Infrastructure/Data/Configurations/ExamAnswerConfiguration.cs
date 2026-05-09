using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class ExamAnswerConfiguration : IEntityTypeConfiguration<ExamAnswer>
{
    public void Configure(EntityTypeBuilder<ExamAnswer> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => new { a.ExamAttemptId, a.QuestionId });

        builder.HasOne(a => a.ExamAttempt).WithMany(at => at.Answers).HasForeignKey(a => a.ExamAttemptId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.Question).WithMany(q => q.Answers).HasForeignKey(a => a.QuestionId).OnDelete(DeleteBehavior.Cascade);
    }
}
