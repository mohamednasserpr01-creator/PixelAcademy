using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class ContentItemConfiguration : IEntityTypeConfiguration<ContentItem>
{
    public void Configure(EntityTypeBuilder<ContentItem> builder)
    {
        builder.HasKey(ci => ci.Id);
        builder.Property(ci => ci.Title).HasMaxLength(200).IsRequired();
        builder.Property(ci => ci.Description).HasMaxLength(2000);
        builder.Property(ci => ci.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(ci => ci.ExternalUrl).HasMaxLength(1000);
        builder.HasIndex(ci => new { ci.LectureId, ci.OrderIndex });
        builder.HasQueryFilter(ci => !ci.IsDeleted);

        builder.HasOne(ci => ci.Lecture)
            .WithMany(l => l.ContentItems)
            .HasForeignKey(ci => ci.LectureId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.MediaAsset)
            .WithOne(ma => ma.ContentItem)
            .HasForeignKey<ContentItem>(ci => ci.MediaAssetId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
