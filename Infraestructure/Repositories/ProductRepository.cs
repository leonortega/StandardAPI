using Dapper;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using StandardAPI.Domain.Entities;
using StandardAPI.Domain.Interfaces;
using StandardAPI.Infraestructure.Persistence;
using StandardAPI.Infraestructure.Services;

namespace StandardAPI.Infraestructure.Repositories
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository(
            ResilientPolicyExecutor policyExecutor,
            RedisCacheService cacheService,
            DatabaseConnectionFactory connectionFactory,
            ILogger<ProductRepository> logger)
            : base(policyExecutor, cacheService, connectionFactory, logger)
        {
        }

        public async Task AddAsync(Product entity)
        {
            const string sql = "INSERT INTO Products (Id, Name, Price) VALUES (@Id, @Name, @Price)";
            await ExecuteWithPolicyAsync(async connection =>
            {
                await connection.ExecuteAsync(sql, entity);
            }, nameof(AddAsync));

            await InvalidateCacheAsync("products:all");
        }

        public async Task DeleteAsync(Guid id)
        {
            const string sql = "DELETE FROM Products WHERE Id = @Id";
            await ExecuteWithPolicyAsync(async connection =>
            {
                await connection.ExecuteAsync(sql, new { Id = id });
            }, nameof(DeleteAsync));

            await InvalidateCacheAsync("products:all");
            await InvalidateCacheAsync($"products:{id}");
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Products";
            return await ExecuteWithPolicyAndCacheAsync("products:all", async connection =>
            {
                var products = await connection.QueryAsync<Product>(sql);
                return products.ToList();
            }, nameof(GetAllAsync), TimeSpan.FromMinutes(15));
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            const string sql = "SELECT * FROM Products WHERE Id = @Id";
            return await ExecuteWithPolicyAndCacheAsync($"products:{id}", async connection =>
            {
                return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
            }, nameof(GetByIdAsync), TimeSpan.FromMinutes(10));
        }

        public async Task UpdateAsync(Product entity)
        {
            const string sql = "UPDATE Products SET Name = @Name, Price = @Price WHERE Id = @Id";
            await ExecuteWithPolicyAsync(async connection =>
            {
                await connection.ExecuteAsync(sql, entity);
            }, nameof(UpdateAsync));

            await InvalidateCacheAsync($"products:{entity.Id}");
            await InvalidateCacheAsync("products:all");
        }

        public async Task<IReadOnlyList<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            string cacheKey = $"products:price:{minPrice}:{maxPrice}";
            const string sql = "SELECT * FROM Products WHERE Price BETWEEN @MinPrice AND @MaxPrice";

            return await ExecuteWithPolicyAndCacheAsync(cacheKey, async connection =>
            {
                var products = await connection.QueryAsync<Product>(sql, new { MinPrice = minPrice, MaxPrice = maxPrice });
                return products.ToList();
            }, nameof(GetProductsByPriceRangeAsync), TimeSpan.FromMinutes(10));
        }
    }
}
