using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class LectureAccessConfiguration : IEntityTypeConfiguration<LectureAccess>
{
    public void Configure(EntityTypeBuilder<LectureAccess> builder)
    {
        builder.HasKey(la => la.Id);
        builder.HasIndex(la => new { la.StudentId, la.LectureId }).IsUnique();

        builder.HasOne(la => la.Student).WithMany(u => u.LectureAccesses).HasForeignKey(la => la.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(la => la.Lecture).WithMany().HasForeignKey(la => la.LectureId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(la => la.ActivationCode).WithMany().HasForeignKey(la => la.ActivationCodeId).OnDelete(DeleteBehavior.SetNull);
    }
}
