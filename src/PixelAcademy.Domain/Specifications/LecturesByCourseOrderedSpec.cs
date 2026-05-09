using System;
using System.Linq;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Specifications;

public class LecturesByCourseOrderedSpec : BaseSpecification<Lecture>
{
    public LecturesByCourseOrderedSpec(Guid courseId)
    {
        Criteria = l => l.CourseId == courseId && !l.IsDeleted;
        AddInclude(l => l.MediaAssets);
        ApplyOrderBy(l => l.OrderIndex);
        ApplyNoTracking();
    }
}
