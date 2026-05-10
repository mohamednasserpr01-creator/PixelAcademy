using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.PhoneNumber).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();
        builder.Property(u => u.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(u => u.Username).HasMaxLength(50).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(u => u.FullName).HasMaxLength(200).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.ParentPhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(u => u.Governorate).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Address).HasMaxLength(500).IsRequired();
        builder.Property(u => u.SchoolName).HasMaxLength(200).IsRequired();
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(50);
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);
        builder.Property(u => u.Bio).HasMaxLength(2000);
        builder.Property(u => u.WalletBalance).HasPrecision(18, 2).HasDefaultValue(0);
        builder.HasQueryFilter(u => !u.IsDeleted);

        builder.HasOne(u => u.EducationalStage)
            .WithMany()
            .HasForeignKey(u => u.EducationalStageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.EducationStream)
            .WithMany()
            .HasForeignKey(u => u.EducationStreamId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.Enrollments).WithOne(e => e.Student).HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.OwnedCourses).WithOne(c => c.Instructor).HasForeignKey(c => c.InstructorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(u => u.Sessions).WithOne(s => s.User).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.RefreshTokens).WithOne(rt => rt.User).HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.VideoProgresses).WithOne(vp => vp.Student).HasForeignKey(vp => vp.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.GeneratedCodes).WithOne(ac => ac.GeneratedBy).HasForeignKey(ac => ac.GeneratedById).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(u => u.Transactions).WithOne(t => t.User).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.LectureAccesses).WithOne(la => la.Student).HasForeignKey(la => la.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.WalletTransactions).WithOne(wt => wt.User).HasForeignKey(wt => wt.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
