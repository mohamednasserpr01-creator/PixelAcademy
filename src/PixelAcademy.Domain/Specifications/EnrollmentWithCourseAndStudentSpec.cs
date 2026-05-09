using System;
using System.Linq;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Specifications;

public class EnrollmentWithCourseAndStudentSpec : BaseSpecification<Enrollment>
{
    public EnrollmentWithCourseAndStudentSpec(Guid studentId, Guid courseId)
    {
        Criteria = e => e.StudentId == studentId && e.CourseId == courseId;
        AddInclude(e => e.Student);
        AddInclude(e => e.Course);
    }
}
