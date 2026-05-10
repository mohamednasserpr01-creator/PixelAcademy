using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class EducationStreamConfiguration : IEntityTypeConfiguration<EducationStream>
{
    public void Configure(EntityTypeBuilder<EducationStream> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.HasOne(e => e.EducationalStage)
            .WithMany(s => s.EducationStreams)
            .HasForeignKey(e => e.EducationalStageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
