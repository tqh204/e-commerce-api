using FluentValidation;

namespace WebAPI.Middlewares;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.GroupBy(e => e.PropertyName).ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await Results.ValidationProblem(errors).ExecuteAsync(context);
        }
    }
}
//This is a middleware that catches any ValidationException thrown in the application and returns a standardized validation problem response to the client.
//It groups the validation errors by property name and formats them in a way that can be easily consumed by the client.