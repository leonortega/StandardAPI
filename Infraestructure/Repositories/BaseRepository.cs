using System.Data;
using Microsoft.Extensions.Logging;
using StandardAPI.Infraestructure.Persistence;
using StandardAPI.Infraestructure.Services;

namespace Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        private readonly ResilientPolicyExecutor _policyExecutor;
        private readonly RedisCacheService _cacheService;
        private readonly DatabaseConnectionFactory _connectionFactory;
        private readonly ILogger<BaseRepository> _logger;

        protected BaseRepository(
            ResilientPolicyExecutor policyExecutor,
            RedisCacheService cacheService,
            DatabaseConnectionFactory connectionFactory,
            ILogger<BaseRepository> logger)
        {
            _policyExecutor = policyExecutor;
            _cacheService = cacheService;
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        /// <summary>
        /// Provides a database connection for operations.
        /// </summary>
        protected IDbConnection CreateConnection()
        {
            return _connectionFactory.CreateConnection();
        }

        /// <summary>
        /// Executes a database operation with caching and Polly resilience policies.
        /// </summary>
        /// <typeparam name="T">The type of the data being retrieved.</typeparam>
        /// <param name="cacheKey">The Redis cache key.</param>
        /// <param name="operation">The database operation to execute.</param>
        /// <param name="policyKey">The Polly policy key to apply.</param>
        /// <param name="cacheDuration">Optional cache duration (default: 10 minutes).</param>
        /// <returns>The result of the operation.</returns>
        protected async Task<T> ExecuteWithPolicyAndCacheAsync<T>(
            string cacheKey,
            Func<IDbConnection, Task<T>> operation,
            string policyKey,
            TimeSpan? cacheDuration = null)
        {
            try
            {
                // Check the cache
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    var cachedData = await _cacheService.GetAsync<T>(cacheKey);
                    if (cachedData != null)
                    {
                        _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                        return cachedData;
                    }
                }

                // Execute the operation with Polly resilience
                var result = await _policyExecutor.ExecuteAsync(() =>
                {
                    using var connection = CreateConnection();
                    return operation(connection);
                }, policyKey);

                // Store the result in the cache
                if (!string.IsNullOrWhiteSpace(cacheKey) && result != null)
                {
                    await _cacheService.SetAsync(cacheKey, result);
                    _logger.LogInformation("Data cached for key: {CacheKey} with duration: {CacheDuration}", cacheKey, cacheDuration);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during operation with cache key: {CacheKey}", cacheKey);
                throw;
            }
        }

        /// <summary>
        /// Executes a database operation with Polly resilience policies.
        /// </summary>
        /// <param name="operation">The database operation to execute.</param>
        /// <param name="policyKey">The Polly policy key to apply.</param>
        protected async Task ExecuteWithPolicyAsync(Func<IDbConnection, Task> operation, string policyKey)
        {
            try
            {
                await _policyExecutor.ExecuteAsync(() =>
                {
                    using var connection = CreateConnection();
                    return operation(connection);
                }, policyKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during operation with Polly policy: {PolicyKey}", policyKey);
                throw;
            }
        }

        /// <summary>
        /// Invalidates a cache entry by key.
        /// </summary>
        /// <param name="cacheKey">The Redis cache key to invalidate.</param>
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
