using System;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Specifications;

public class ActiveSessionByUserSpec : BaseSpecification<Session>
{
    public ActiveSessionByUserSpec(Guid userId)
    {
        Criteria = s => s.UserId == userId && !s.IsRevoked && !s.IsExpired;
    }
}
