using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(q => q.Id);
        builder.HasIndex(q => q.ExamId);

        builder.HasOne(q => q.Exam).WithMany(e => e.Questions).HasForeignKey(q => q.ExamId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(q => !q.IsDeleted);
    }
}
