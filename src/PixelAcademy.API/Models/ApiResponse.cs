using System;
using System.Collections.Generic;

namespace PixelAcademy.API.Models;

public class ApiResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; }
    public string? TraceId { get; set; }
    public PaginationMeta? Pagination { get; set; }

    public ApiResponse()
    {
        Timestamp = DateTime.UtcNow;
    }

    public static ApiResponse SuccessResponse(object? data, string? message = null, PaginationMeta? pagination = null)
    {
        return new ApiResponse
        {
            Success = true,
            Data = data,
            Message = message,
            Pagination = pagination,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse ErrorResponse(List<string> errors, string? message = null)
    {
        return new ApiResponse
        {
            Success = false,
            Data = null,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

public class PaginationMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }

    public static PaginationMeta? FromPagedResult<T>(PixelAcademy.Application.DTOs.Common.PagedResult<T>? pagedResult)
    {
        if (pagedResult == null) return null;
        return new PaginationMeta
        {
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalCount = pagedResult.TotalCount,
            TotalPages = pagedResult.TotalPages,
            HasNextPage = pagedResult.HasNextPage,
            HasPreviousPage = pagedResult.HasPreviousPage
        };
    }
}

public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
}
