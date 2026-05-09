using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.CourseId);
        builder.HasIndex(a => a.CreatedById);
        builder.HasIndex(a => a.IsPublished);
        builder.HasIndex(a => a.ScheduledPublishAt);

        builder.HasOne(a => a.Course).WithMany().HasForeignKey(a => a.CourseId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(a => a.Creator).WithMany(u => u.CreatedAnnouncements).HasForeignKey(a => a.CreatedById).OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
