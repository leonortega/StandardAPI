using System.Data;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;
using StandardAPI.Infraestructure.Persistence;
using StandardAPI.Infraestructure.Services;

namespace StandardAPI.Infraestructure.Repositories
{
    public abstract class BaseRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;
        private readonly AsyncPolicyWrap _resiliencePolicy;
        private readonly RedisCacheService _cacheService;
        private readonly ILogger<BaseRepository> _logger;

        protected BaseRepository(
            DatabaseConnectionFactory connectionFactory,
            AsyncRetryPolicy retryPolicy,
            AsyncCircuitBreakerPolicy circuitBreakerPolicy,
            RedisCacheService cacheService,
            ILogger<BaseRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _resiliencePolicy = retryPolicy.WrapAsync(circuitBreakerPolicy);
            _cacheService = cacheService;
            _logger = logger;
        }

        protected async Task<T> ExecuteWithPolicyAndCacheAsync<T>(
            string cacheKey,
            Func<IDbConnection, Task<T>> operation,
            string operationName,
            TimeSpan? cacheDuration = null)
        {
            try
            {
                // Check cache first
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    var cachedResult = await _cacheService.GetAsync<T>(cacheKey);
                    if (cachedResult != null)
                    {
                        _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                        return cachedResult;
                    }
                }

                // Execute operation with Polly resilience
                using var connection = _connectionFactory.CreateConnection();
                var result = await _resiliencePolicy.ExecuteAsync(() =>
                {
                    _logger.LogInformation("Executing database operation: {OperationName}", operationName);
                    return operation(connection);
                });

                // Cache the result
                if (!string.IsNullOrWhiteSpace(cacheKey) && result != null)
                {
                    await _cacheService.SetAsync(cacheKey, result);
                    _logger.LogInformation("Result cached for key: {CacheKey} with duration: {CacheDuration}", cacheKey, cacheDuration);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database operation: {OperationName}", operationName);
                throw;
            }
        }

        protected async Task ExecuteWithPolicyAsync(Func<IDbConnection, Task> operation, string operationName)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await _resiliencePolicy.ExecuteAsync(() =>
                {
                    _logger.LogInformation("Executing database operation: {OperationName}", operationName);
                    return operation(connection);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database operation: {OperationName}", operationName);
                throw;
            }
        }

        protected async Task InvalidateCacheAsync(string cacheKey)
        {
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                await _cacheService.DeleteAsync(cacheKey);
                _logger.LogInformation("Cache invalidated for key: {CacheKey}", cacheKey);
            }
        }
    }
}
