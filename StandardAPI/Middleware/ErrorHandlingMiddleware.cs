using System.Text.Json;

namespace StandardAPI.API.Middleware
{
    public class ErrorHandlingMiddleware(RequestDelegate next, ILogger log)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An unexpected error occurred.");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var response = new { message = "An error occurred while processing your request." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
