using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PixelAcademy.API.Models;
using PixelAcademy.Application.DTOs.Common;

namespace PixelAcademy.API.Filters;

public class ApiResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value != null)
        {
            var statusCode = objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;

            // Don't wrap if already wrapped, or if it's a file/redirect, or if error status
            if (statusCode >= 400 || objectResult.Value is ApiResponse || objectResult.Value is ProblemDetails)
            {
                await next();
                return;
            }

            var traceId = context.HttpContext.TraceIdentifier;

            // Handle PagedResult/PagedResponse separately to include pagination metadata
            if (IsPagedType(objectResult.Value, out var items, out var pagination))
            {
                var wrapped = ApiResponse.SuccessResponse(items, pagination: pagination);
                wrapped.TraceId = traceId;
                context.Result = new ObjectResult(wrapped) { StatusCode = statusCode };
                await next();
                return;
            }

            var wrapper = ApiResponse.SuccessResponse(objectResult.Value);
            wrapper.TraceId = traceId;
            context.Result = new ObjectResult(wrapper) { StatusCode = statusCode };
        }

        await next();
    }

    private static bool IsPagedType(object value, out object? items, out PaginationMeta? pagination)
    {
        items = null;
        pagination = null;

        var type = value.GetType();
        if (!type.IsGenericType) return false;

        var genericDef = type.GetGenericTypeDefinition();
        if (genericDef == typeof(PixelAcademy.Application.DTOs.Common.PagedResult<>))
        {
            items = ExtractProperty(value, "Items");
            pagination = new PaginationMeta
            {
                Page = (int)ExtractPropertyValue(value, "Page")!,
                PageSize = (int)ExtractPropertyValue(value, "PageSize")!,
                TotalCount = (int)ExtractPropertyValue(value, "TotalCount")!,
                TotalPages = (int)ExtractPropertyValue(value, "TotalPages")!,
                HasNextPage = (bool)ExtractPropertyValue(value, "HasNextPage")!,
                HasPreviousPage = (bool)ExtractPropertyValue(value, "HasPreviousPage")!
            };
            return true;
        }

        if (genericDef == typeof(PixelAcademy.Application.Abstractions.Pagination.PagedResponse<>))
        {
            items = ExtractProperty(value, "Items");
            pagination = new PaginationMeta
            {
                Page = (int)ExtractPropertyValue(value, "PageNumber")!,
                PageSize = (int)ExtractPropertyValue(value, "PageSize")!,
                TotalCount = (int)ExtractPropertyValue(value, "TotalCount")!,
                TotalPages = (int)ExtractPropertyValue(value, "TotalPages")!,
                HasNextPage = (bool)ExtractPropertyValue(value, "HasNextPage")!,
                HasPreviousPage = (bool)ExtractPropertyValue(value, "HasPreviousPage")!
            };
            return true;
        }

        return false;
    }

    private static object? ExtractProperty(object value, string propertyName)
    {
        var prop = value.GetType().GetProperty(propertyName);
        return prop?.GetValue(value);
    }

    private static object? ExtractPropertyValue(object value, string propertyName)
    {
        var prop = value.GetType().GetProperty(propertyName);
        return prop?.GetValue(value);
    }
}
