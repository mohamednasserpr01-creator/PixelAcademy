using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Specifications;

public class UserByPhoneNumberSpec : BaseSpecification<User>
{
    public UserByPhoneNumberSpec(string phoneNumber)
    {
        Criteria = u => u.PhoneNumber == phoneNumber;
    }
}
