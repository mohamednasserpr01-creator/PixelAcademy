using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Course> Courses { get; }
    DbSet<Lecture> Lectures { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<Session> Sessions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<VideoProgress> VideoProgresses { get; }
    DbSet<MediaAsset> MediaAssets { get; }
    DbSet<ContentItem> ContentItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
