using Microsoft.AspNetCore.Builder;
using Music.Services.Exceptions.Middlewares;

namespace Music.Services.Exceptions.Helpers;

public static class ExceptionMiddlewareHelper
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<GlobalExceptionMiddleware>();
        return builder;
    }
}