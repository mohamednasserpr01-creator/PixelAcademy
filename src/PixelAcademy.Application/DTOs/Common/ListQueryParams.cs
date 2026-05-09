using System.ComponentModel.DataAnnotations;

namespace PixelAcademy.Application.DTOs.Common;

public class ListQueryParams
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;

    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "desc";
    public string? Filter { get; set; }
}
