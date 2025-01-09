using System.Text.Json;
using StackExchange.Redis;

namespace StandardAPI.Infraestructure.Services
{
    public class RedisCacheService
    {
        private readonly IDatabase _database;
        private readonly int _defaultExpiryMinutes;

        public RedisCacheService(IConnectionMultiplexer redis, int defaultExpiryMinutes = 10)
        {
            _database = redis.GetDatabase();
            _defaultExpiryMinutes = defaultExpiryMinutes;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, serializedValue, expiration);
        }

        public async Task DeleteAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }
    }
}
