using BLL.DTOs.Responses;
using BLL.Exceptions;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibraryWebApp_v2;

public class Middleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<Middleware> _logger;
    private readonly bool _isDevelopment;

    public Middleware(
        RequestDelegate next,
        ILogger<Middleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _isDevelopment = env.IsDevelopment();
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = GetStatusCodeAndMessage(exception);

        _logger.LogError(exception, "Ошибка обработки запроса: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            Details = _isDevelopment ? exception.Message : null,
            StackTrace = _isDevelopment ? exception.StackTrace : null
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }));
    }

    private (HttpStatusCode statusCode, string message) GetStatusCodeAndMessage(Exception exception)
    {
        return exception switch
        {
            ValidationException => (HttpStatusCode.BadRequest, exception.Message),
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            UnauthorizedException => (HttpStatusCode.Unauthorized, exception.Message),
        };
    }
}