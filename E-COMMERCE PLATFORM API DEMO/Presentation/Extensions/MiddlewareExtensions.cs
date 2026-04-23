using WebAPI.Middlewares;

namespace Presentation.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseValidationExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ValidationExceptionMiddleware>();
    }
}
// This extension method allows you to easily add the ValidationExceptionMiddleware to the application's middleware pipeline by calling app.UseValidationExceptionHandler() in the Program.cs file.