using PixelAcademy.Application.DTOs.Lectures;

namespace PixelAcademy.Application.DTOs.Courses;

public class CourseDetailDto : CourseDto
{
    public List<LectureDto> Lectures { get; set; } = new();
    public bool IsEnrolled { get; set; }
    public decimal? UserProgressPercent { get; set; }
}
