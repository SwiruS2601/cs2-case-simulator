using Cs2CaseOpener.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Cs2CaseOpener.Extensions;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}