using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Lectures;

namespace PixelAcademy.Application.Queries.Lectures;

public class GetLectureByIdQuery : IQuery<LectureDto>
{
    public Guid Id { get; set; }
}
