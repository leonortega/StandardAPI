using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Retry;
using StandardAPI.Domain.Entities;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Infraestructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(IDistributedCache cache,
                                 ILogger<ProductRepository> logger,
                                 AsyncRetryPolicy retryPolicy,
                                 AsyncCircuitBreakerPolicy circuitBreakerPolicy,
                                 string connectionString)
            : base(cache, logger, retryPolicy, circuitBreakerPolicy, connectionString)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            string sql = @"
            SELECT * 
            FROM product
            WHERE Price >= @MinPrice AND Price <= @MaxPrice";
            return await QueryAsync<Product>(sql, new { MinPrice = minPrice, MaxPrice = maxPrice });
        }
    }
}