using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Music.Services.Exceptions.Exceptions;

namespace Music.Services.Exceptions.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;


    public GlobalExceptionMiddleware(RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            var statusCode = 500;
            var message = "We made a mistake, but we are working on it!";
            
            if (exception is ExceptionBase ex)
            {
                message = ex.Message;
                statusCode = (int)ex.StatusCode;
                
                _logger.LogInformation(
                    "Could not process a request on machine {Machine}. " +
                    "Status Code: {StatusCode}. " +
                    "Message: {Message}",
                    Environment.MachineName,
                    statusCode,
                    message);
            }
            else
            {
                _logger.LogError(exception, 
                    "Could not process a request on machine {Machine}. TraceId: {TraceId}",
                    Environment.MachineName,
                    Activity.Current?.Id);
            }
            
            await Results.Problem(
                message,
                statusCode:  statusCode,
                extensions: new Dictionary<string, object?>
                {
                    {"traceId", Activity.Current?.Id}
                }
            ).ExecuteAsync(httpContext);
        }
    }
}