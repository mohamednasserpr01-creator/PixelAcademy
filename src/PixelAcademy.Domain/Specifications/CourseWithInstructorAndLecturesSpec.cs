using System;
using System.Linq;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Specifications;

public class CourseWithInstructorAndLecturesSpec : BaseSpecification<Course>
{
    public CourseWithInstructorAndLecturesSpec(Guid courseId)
    {
        Criteria = c => c.Id == courseId;
        AddInclude(c => c.Instructor);
        AddInclude(c => c.Lectures.OrderBy(l => l.OrderIndex));
        AddInclude("Lectures.MediaAssets");
        AddInclude(c => c.MediaAssets);
    }
}
