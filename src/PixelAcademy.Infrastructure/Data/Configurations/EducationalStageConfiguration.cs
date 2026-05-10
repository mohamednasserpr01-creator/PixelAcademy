using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class EducationalStageConfiguration : IEntityTypeConfiguration<EducationalStage>
{
    public void Configure(EntityTypeBuilder<EducationalStage> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.HasMany(e => e.EducationStreams)
            .WithOne(s => s.EducationalStage)
            .HasForeignKey(s => s.EducationalStageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
