using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.HasKey(wt => wt.Id);
        builder.Property(wt => wt.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(wt => wt.Amount).HasPrecision(18, 2);
        builder.Property(wt => wt.BalanceBefore).HasPrecision(18, 2);
        builder.Property(wt => wt.BalanceAfter).HasPrecision(18, 2);
        builder.Property(wt => wt.Description).HasMaxLength(500);

        builder.HasOne(wt => wt.User).WithMany(u => u.WalletTransactions).HasForeignKey(wt => wt.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(wt => wt.ActivationCode).WithMany().HasForeignKey(wt => wt.ActivationCodeId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(wt => wt.Course).WithMany().HasForeignKey(wt => wt.CourseId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(wt => wt.Lecture).WithMany().HasForeignKey(wt => wt.LectureId).OnDelete(DeleteBehavior.SetNull);
    }
}
