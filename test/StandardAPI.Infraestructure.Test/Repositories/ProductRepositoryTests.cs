using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using StandardAPI.Domain.Entities;
using StandardAPI.Infraestructure.Repositories;
using Xunit;

namespace StandardAPI.Infraestructure.Test.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly Mock<ILogger<ProductRepository>> _loggerMock;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly string _connectionString;

        public ProductRepositoryTests()
        {
            _cacheMock = new Mock<IDistributedCache>();
            _loggerMock = new Mock<ILogger<ProductRepository>>();
            _retryPolicy = Policy.Handle<Exception>().RetryAsync(2); // Retry 2 times
            _circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromMinutes(1));
            _connectionString = "YourConnectionString";
        }

        [Fact]
        public async Task GetProductsByPriceRangeAsyncReturnsProducts()
        {
            // Arrange
            var minPrice = 10m;
            var maxPrice = 100m;
            var expectedProducts = new List<Product>
            {
                new() { Id = Guid.NewGuid(), Name = "Product1", Price = 20m },
                new() { Id = Guid.NewGuid(), Name = "Product2", Price = 50m }
            };

            var baseRepositoryMock = new Mock<BaseRepository<Product>>(
                _cacheMock.Object,
                _loggerMock.Object,
                _retryPolicy,
                _circuitBreakerPolicy,
                _connectionString);

            baseRepositoryMock
                .Setup(repo => repo.QueryAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(expectedProducts);

            var productRepository = new TestableProductRepository(
                _cacheMock.Object,
                _loggerMock.Object,
                _retryPolicy,
                _circuitBreakerPolicy,
                _connectionString,
                baseRepositoryMock.Object);

            // Act
            var products = await productRepository.GetProductsByPriceRangeAsync(minPrice, maxPrice);

            // Assert
            Assert.NotNull(products);
            Assert.Equal(expectedProducts.Count, products.Count());
            Assert.Equal(expectedProducts, products);
        }

        [Fact]
        public async Task GetProductsByPriceRangeAsyncRetriesOnFailure()
        {
            // Arrange
            var minPrice = 10m;
            var maxPrice = 100m;
            var expectedProducts = new List<Product>
            {
                new() { Id = Guid.NewGuid(), Name = "Product1", Price = 20m },
                new() { Id = Guid.NewGuid(), Name = "Product2", Price = 50m }
            };

            var baseRepositoryMock = new Mock<BaseRepository<Product>>(
                _cacheMock.Object,
                _loggerMock.Object,
                _retryPolicy,
                _circuitBreakerPolicy,
                _connectionString);

            int retryCount = 0;
            baseRepositoryMock
                .Setup(repo => repo.QueryAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(() =>
                {
                    if (retryCount++ < 4)
                    {
                        throw new TimeoutException("Transient error");
                    }
                    return expectedProducts;
                });

            var productRepository = new TestableProductRepository(
                _cacheMock.Object,
                _loggerMock.Object,
                _retryPolicy,
                _circuitBreakerPolicy,
                _connectionString,
                baseRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<TimeoutException>(() => productRepository.GetProductsByPriceRangeAsync(minPrice, maxPrice));
            Assert.Equal(1, retryCount);
        }

        [Fact]
        public async Task GetProductsByPriceRangeAsyncOpensCircuitBreakerOnFailure()
        {
            // Arrange
            var minPrice = 10m;
            var maxPrice = 100m;

            var baseRepositoryMock = new Mock<BaseRepository<Product>>(
                _cacheMock.Object,
                _loggerMock.Object,
                _retryPolicy,
                _circuitBreakerPolicy,
                _connectionString);

            baseRepositoryMock
                .Setup(repo => repo.QueryAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                .ThrowsAsync(new BrokenCircuitException("Persistent error"));

            var productRepository = new TestableProductRepository(
                _cacheMock.Object,
                _loggerMock.Object,
                _retryPolicy,
                _circuitBreakerPolicy,
                _connectionString,
                baseRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BrokenCircuitException>(() => productRepository.GetProductsByPriceRangeAsync(minPrice, maxPrice));
        }

        internal sealed class TestableProductRepository(IDistributedCache cache,
                                         ILogger<ProductRepository> logger,
                                         AsyncRetryPolicy retryPolicy,
                                         AsyncCircuitBreakerPolicy circuitBreakerPolicy,
                                         string connectionString,
                                         BaseRepository<Product> baseRepository) : ProductRepository(cache, logger, retryPolicy, circuitBreakerPolicy, connectionString)
        {
            private readonly BaseRepository<Product> _baseRepository = baseRepository;

            public override async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
            {
                return await _baseRepository.QueryAsync<T>(sql, param);
            }
        }
    }
}
