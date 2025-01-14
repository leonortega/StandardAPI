using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly.CircuitBreaker;
using Polly.Retry;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Infraestructure.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<BaseRepository<TEntity>> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly string _connectionString;

        public BaseRepository(IDistributedCache cache,
                              ILogger<BaseRepository<TEntity>> logger,
                              AsyncRetryPolicy retryPolicy,
                              AsyncCircuitBreakerPolicy circuitBreakerPolicy,
                              string connectionString)
        {
            _cache = cache;
            _logger = logger;
            _retryPolicy = retryPolicy;
            _circuitBreakerPolicy = circuitBreakerPolicy;
            _connectionString = connectionString;
        }

        private async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await connection.ExecuteScalarAsync<T>(sql, param);
                }
            });
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await connection.QueryAsync<T>(sql, param);
                }
            });
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            string cacheKey = $"All_{typeof(TEntity).Name}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Retrieving all {EntityName} from cache.", typeof(TEntity).Name);
                return JsonSerializer.Deserialize<IEnumerable<TEntity>>(cachedData) ?? [];
            }

            _logger.LogInformation("Retrieving all {EntityName} from database.", typeof(TEntity).Name);
            string tableName = typeof(TEntity).Name;
            string sql = $"SELECT * FROM {tableName}";
            var entities = await QueryAsync<TEntity>(sql);

            var serializedEntities = JsonSerializer.Serialize(entities);
            await _cache.SetStringAsync(cacheKey, serializedEntities);

            return entities;
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            string cacheKey = $"{typeof(TEntity).Name}_{id}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Retrieving {EntityName} with ID {Id} from cache.", typeof(TEntity).Name, id);
                return JsonSerializer.Deserialize<TEntity>(cachedData);
            }

            _logger.LogInformation("Retrieving {EntityName} with ID {Id} from database.", typeof(TEntity).Name, id);
            string tableName = typeof(TEntity).Name;
            string sql = $"SELECT * FROM {tableName} WHERE Id = @Id";
            var entity = (await QueryAsync<TEntity>(sql, new { Id = id })).FirstOrDefault();

            if (entity != null)
            {
                var serializedEntity = JsonSerializer.Serialize(entity);
                await _cache.SetStringAsync(cacheKey, serializedEntity);
            }

            return entity;
        }

        public virtual async Task<int> AddAsync(TEntity entity)
        {
            string tableName = typeof(TEntity).Name;
            // Get the properties of the entity
            var properties = typeof(TEntity).GetProperties();
            // Construct the column names and parameter names
            string columns = string.Join(",", properties.Select(p => $"[{p.Name}]"));
            string parameters = string.Join(",", properties.Select(p => $"@{p.Name}"));
            string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters}); SELECT CAST(SCOPE_IDENTITY() as int)";

            var result = await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                return await ExecuteScalarAsync<int>(sql, entity);
            });

            // Invalidate cache after adding a new entity
            string allEntitiesCacheKey = $"All_{typeof(TEntity).Name}";
            await _cache.RemoveAsync(allEntitiesCacheKey);

            return result;
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            string tableName = typeof(TEntity).Name;
            // Get the properties of the entity
            var properties = typeof(TEntity).GetProperties();
            // Construct the update set clause
            string setClause = string.Join(",", properties.Select(p => $"[{p.Name}] = @{p.Name}"));
            string sql = $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";

            var result = await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                return await ExecuteScalarAsync<int>(sql, entity);
            });

            // Invalidate cache after updating an entity
            string allEntitiesCacheKey = $"All_{typeof(TEntity).Name}";
            string entityCacheKey = $"{typeof(TEntity).Name}_{((dynamic)entity).Id}";
            await _cache.RemoveAsync(allEntitiesCacheKey);
            await _cache.RemoveAsync(entityCacheKey);

            return result;
        }

        public virtual async Task<int> DeleteAsync(Guid id)
        {
            string tableName = typeof(TEntity).Name;
            string sql = $"DELETE FROM {tableName} WHERE Id = @Id";

            var result = await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                return await ExecuteScalarAsync<int>(sql, new { Id = id });
            });

            // Invalidate cache after deleting an entity
            string allEntitiesCacheKey = $"All_{typeof(TEntity).Name}";
            string entityCacheKey = $"{typeof(TEntity).Name}_{id}";
            await _cache.RemoveAsync(allEntitiesCacheKey);
            await _cache.RemoveAsync(entityCacheKey);

            return result;
        }
    }
}