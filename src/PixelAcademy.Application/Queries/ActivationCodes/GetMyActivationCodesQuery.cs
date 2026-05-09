using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.ActivationCodes;

namespace PixelAcademy.Application.Queries.ActivationCodes;

public class GetMyActivationCodesQuery : IQuery<PagedResponse<ActivationCodeDto>>
{
    public Guid UserId { get; set; }
    public bool AsGenerator { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
