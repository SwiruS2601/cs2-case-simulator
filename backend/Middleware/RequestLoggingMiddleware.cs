using System.Diagnostics;

namespace Cs2CaseOpener.Middleware;
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var ipAddress = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault()
            ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers.UserAgent.FirstOrDefault();

        try
        {
            _logger.LogInformation(
                "Request: {Method} {Path} | IP: {IP} | User-Agent: {UserAgent} | CorrelationId: {CorrelationId}",
                context.Request.Method, context.Request.Path, ipAddress, userAgent, context.TraceIdentifier);

            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "Response: {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | IP: {IP} | User-Agent: {UserAgent} | CorrelationId: {CorrelationId}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds, ipAddress, userAgent, context.TraceIdentifier);
        }
    }
}