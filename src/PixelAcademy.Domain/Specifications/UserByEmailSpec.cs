using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Specifications;

public class UserByEmailSpec : BaseSpecification<User>
{
    public UserByEmailSpec(string email)
    {
        Criteria = u => u.Email == email;
    }
}
