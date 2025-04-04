using Newtonsoft.Json;
using System.Net;

namespace LibraryWebApp_v2;

public class Middleware
{
    private readonly RequestDelegate _next;

    public Middleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Console.WriteLine($"Произошла ошибка: {exception.Message}");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            context.Response.StatusCode,
            Message = "Произошла непредвиденная ошибка. Попробуйте позже.",
            Detailed = exception.Message
        };

        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}
