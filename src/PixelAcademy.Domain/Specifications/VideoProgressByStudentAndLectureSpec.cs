using System;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Specifications;

public class VideoProgressByStudentAndLectureSpec : BaseSpecification<VideoProgress>
{
    public VideoProgressByStudentAndLectureSpec(Guid studentId, Guid lectureId)
    {
        Criteria = vp => vp.StudentId == studentId && vp.LectureId == lectureId;
        AddInclude(vp => vp.Lecture);
    }
}
