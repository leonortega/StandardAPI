using System.Net;
using System.Text.Json;
using Azure.Core;

namespace StandardAPI.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _log;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> log)
        {
            _next = next;
            _log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                await _next(context);
            }
            catch (HttpRequestException ex)
            {
                _log.LogError(ex, "An HTTP request error occurred.");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 503;

                var response = new { message = "A service error occurred while processing your request." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (WebException ex)
            {
                _log.LogError(ex, "A web error occurred.");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 502;

                var response = new { message = "A web error occurred while processing your request." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (InvalidOperationException ex)
            {
                _log.LogError(ex, "An invalid operation error occurred.");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;

                var response = new { message = "An invalid operation occurred while processing your request." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
