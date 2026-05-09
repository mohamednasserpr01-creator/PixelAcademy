using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Specifications;

public class ActiveRefreshTokenSpec : BaseSpecification<RefreshToken>
{
    public ActiveRefreshTokenSpec(string token)
    {
        Criteria = rt => rt.Token == token;
        AddInclude(rt => rt.User);
    }
}
