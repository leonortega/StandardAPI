using Microsoft.Extensions.Logging;
using Moq;
using StandardAPI.Application.DTOs;
using StandardAPI.Application.Interfaces;
using StandardAPI.Application.UseCases.Handlers;
using StandardAPI.Application.UseCases.Queries;
using StandardAPI.Domain.Entities;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Application.Tests.Handlers
{
    public class GetProductByIdQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IProductMapper> _mapperMock;
        private readonly Mock<ILogger<GetProductByIdQueryHandler>> _loggerMock;
        private readonly GetProductByIdQueryHandler _handler;

        public GetProductByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IProductMapper>();
            _loggerMock = new Mock<ILogger<GetProductByIdQueryHandler>>();
            _handler = new GetProductByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task HandleShouldReturnProductDtoWhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product", Description = "Test Description", Price = 10.0m };
            var productDto = new ProductDto { Id = productId, Name = "Test Product", Description = "Test Description", Price = 10.0m };

            _repositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.MapToProductDto(product)).Returns(productDto);

            var query = new GetProductByIdQuery(productId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once);
            _mapperMock.Verify(m => m.MapToProductDto(product), Times.Once);
            Assert.Equal(productDto, result);
        }

        [Fact]
        public async Task HandleShouldReturnNullWhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

            var query = new GetProductByIdQuery(productId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once);
            _mapperMock.Verify(m => m.MapToProductDto(It.IsAny<Product>()), Times.Never);
            Assert.Null(result);
        }

        [Fact]
        public async Task HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
        {
            // Arrange
            GetProductByIdQuery? query = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
