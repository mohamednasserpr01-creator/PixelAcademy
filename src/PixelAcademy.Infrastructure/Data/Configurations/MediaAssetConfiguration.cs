using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.FileName).HasMaxLength(255).IsRequired();
        builder.Property(m => m.OriginalFileName).HasMaxLength(255).IsRequired();
        builder.Property(m => m.Url).HasMaxLength(1000).IsRequired();
        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(m => m.MimeType).HasMaxLength(100);

        builder.HasOne(m => m.Course).WithMany(c => c.MediaAssets).HasForeignKey(m => m.CourseId);
        builder.HasOne(m => m.Lecture).WithMany(l => l.MediaAssets).HasForeignKey(m => m.LectureId);
        builder.HasOne(m => m.ContentItem).WithOne(ci => ci.MediaAsset).HasForeignKey<MediaAsset>(m => m.ContentItemId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(m => m.UploadedBy).WithMany().HasForeignKey(m => m.UploadedById);
    }
}
