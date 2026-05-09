using System;

namespace PixelAcademy.Domain.Interfaces;

public interface ISignedUrlService
{
    string GenerateSignedVideoUrl(Guid lectureId, Guid studentId, TimeSpan expiration);
    bool ValidateSignedToken(string token, out Guid lectureId, out Guid studentId);
}
