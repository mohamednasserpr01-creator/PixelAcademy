using System.Linq;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Specifications;

public class CoursePublishedSpec : BaseSpecification<Course>
{
    public CoursePublishedSpec()
    {
        Criteria = c => c.Status == CourseStatus.Published && !c.IsDeleted;
        ApplyNoTracking();
    }
}
