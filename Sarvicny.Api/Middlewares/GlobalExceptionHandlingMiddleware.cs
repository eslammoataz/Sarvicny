using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Sarvicny.Api.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{

    private readonly ILogger _logger;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            var problemDetails = new ProblemDetails
            {
                Status = 500,
                Title = "Internal Server Error",
                Detail = GetExceptionDetails(ex.InnerException),
                Type = ex.GetType().ToString() // Set the actual type of the exception
            };

            // Return the ProblemDetails as JSON
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            LogExceptionDetails(ex);
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    private void LogExceptionDetails(Exception ex)
    {
        // Log exception details including stack trace and inner exceptions
        _logger.LogError(ex, $"Exception Details: {ex.Message}\nStackTrace: {ex.StackTrace}");

        if (ex.InnerException != null)
        {
            LogExceptionDetails(ex.InnerException);
        }
    }

    private string GetExceptionDetails(Exception ex)
    {
        // Return a string with exception details including stack trace and inner exceptions
        if (ex == null)
        {
            return "No inner exception";
        }

        StringBuilder details = new StringBuilder();
        details.AppendLine($"Exception Details: {ex.Message}");
        details.AppendLine($"StackTrace: {ex.StackTrace}");

        if (ex.InnerException != null)
        {
            details.AppendLine("Inner Exception:");
            details.AppendLine(GetExceptionDetails(ex.InnerException));
        }

        return details.ToString();
    }

}