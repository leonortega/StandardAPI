using Polly;
using Polly.RateLimit;
using StandardAPI.Shared.Settings;

namespace StandardAPI.API.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AsyncRateLimitPolicy _rateLimitPolicy;
        public RateLimitMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;

            var pollySettings = new PollySettings();
            configuration?.GetSection("Polly").Bind(pollySettings);

            _rateLimitPolicy = Policy.RateLimitAsync(pollySettings.RateLimitMaxRequest, TimeSpan.FromSeconds(pollySettings.RateLimitTimeWindowInSeconds));
        }
        public async Task InvokeAsync(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                await _rateLimitPolicy.ExecuteAsync(() => _next(context));
            }
            catch (RateLimitRejectedException)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
            }
        }
    }
}