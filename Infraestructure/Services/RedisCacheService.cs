using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StandardAPI.Infraestructure.Settings;

namespace StandardAPI.Infraestructure.Services
{
    public class RedisCacheService : IDistributedCache
    {
        private readonly IDistributedCache _innerCache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly TimeSpan _defaultExpiration;

        public RedisCacheService(Func<IDistributedCache> innerCacheFactory, ILogger<RedisCacheService> logger, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(innerCacheFactory);

            _innerCache = innerCacheFactory();
            _logger = logger;

            var redisSettings = new RedisSettings();
            configuration.GetSection("Redis").Bind(redisSettings);

            _defaultExpiration = TimeSpan.FromMinutes(configuration.GetValue<int>(redisSettings.DefaultCacheExpiryMinutes!, 60));
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            if (options == null)
            {
                options = new DistributedCacheEntryOptions();
            }

            if (!options.AbsoluteExpiration.HasValue)
            {
                options.AbsoluteExpiration = DateTimeOffset.Now.Add(_defaultExpiration);
            }

            _logger.LogDebug("Setting cache key: {Key}", key);
            return _innerCache.SetAsync(key, value, options, token);
        }

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            _logger.LogDebug("Getting cache key: {Key}", key);
            return _innerCache.GetAsync(key, token);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            _logger.LogDebug("Removing cache key: {Key}", key);
            return _innerCache.RemoveAsync(key, token);
        }

        public byte[]? Get(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            throw new NotImplementedException();
        }

        public void Refresh(string key)
        {
            throw new NotImplementedException();
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}