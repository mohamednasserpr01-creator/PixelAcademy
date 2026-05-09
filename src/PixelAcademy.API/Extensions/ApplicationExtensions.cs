using PixelAcademy.API.Middleware;

namespace PixelAcademy.API.Extensions;

public static class ApplicationExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestTimingMiddleware>();
        return app;
    }
}
