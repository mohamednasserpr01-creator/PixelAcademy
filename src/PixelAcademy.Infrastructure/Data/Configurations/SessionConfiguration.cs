using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Token).HasMaxLength(500).IsRequired();
        builder.Property(s => s.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(s => s.IpAddress).HasMaxLength(50);
        builder.Property(s => s.UserAgent).HasMaxLength(500);
        builder.HasIndex(s => s.Token);

        builder.HasOne(s => s.User).WithMany(u => u.Sessions).HasForeignKey(s => s.UserId);
    }
}
