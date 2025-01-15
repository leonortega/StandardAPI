namespace StandardAPI.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string CorrelationIdHeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                // Generate or retrieve the Correlation ID
                var correlationId = context.Request.Headers[CorrelationIdHeaderName];
                if (string.IsNullOrEmpty(correlationId))
                {
                    correlationId = Guid.NewGuid().ToString();
                    context.Request.Headers[CorrelationIdHeaderName] = correlationId;
                }

                // Add Correlation ID to response headers
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers[CorrelationIdHeaderName] = correlationId;
                    return Task.CompletedTask;
                });

                _logger.LogInformation("Handling request");
                await _next(context);
                _logger.LogInformation("Completed request");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling request in CorrelationIdMiddleware.");
                throw;
            }
        }
    }
}
