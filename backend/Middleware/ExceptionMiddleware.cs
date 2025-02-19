using System.Net;
using System.Text.Json;
using Cs2CaseOpener.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is DomainException domainException)
        {
            var statusCode = (int)(domainException?.Status ?? HttpStatusCode.BadRequest);
            
            context.Response.StatusCode = statusCode;

            var response = JsonSerializer.Serialize(
                new 
                { 
                    error = domainException?.Error,
                    message = domainException?.Message ?? "An error occurred",
                    status = statusCode
                });

            return context.Response.WriteAsync(response);
        }

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var genericResponse = JsonSerializer.Serialize(
            new 
            {
                    error = "Exception", 
                    message = "Internal Server Error", 
                    status = 500 
                });

        return context.Response.WriteAsync(genericResponse);
    }
}
