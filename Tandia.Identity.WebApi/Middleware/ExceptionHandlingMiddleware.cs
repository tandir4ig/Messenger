using System.Net;

namespace Tandia.Identity.WebApi.Middleware;

sealed public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(ex.Message, cancellationToken: context.RequestAborted);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(ex.Message, cancellationToken: context.RequestAborted);
            // Логирование ошибки
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
