using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class WatchSessionConfiguration : IEntityTypeConfiguration<WatchSession>
{
    public void Configure(EntityTypeBuilder<WatchSession> builder)
    {
        builder.HasKey(ws => ws.Id);
        builder.HasIndex(ws => new { ws.StudentId, ws.LectureId, ws.StartedAt });
        builder.HasIndex(ws => ws.CourseId);

        builder.HasOne(ws => ws.Student).WithMany().HasForeignKey(ws => ws.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ws => ws.Lecture).WithMany().HasForeignKey(ws => ws.LectureId).OnDelete(DeleteBehavior.Cascade);
    }
}
