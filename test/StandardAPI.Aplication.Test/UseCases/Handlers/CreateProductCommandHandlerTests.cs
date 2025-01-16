using Moq;
using StandardAPI.Application.DTOs;
using StandardAPI.Application.Interfaces;
using StandardAPI.Application.UseCases.Commands;
using StandardAPI.Application.UseCases.Handlers;
using StandardAPI.Domain.Entities;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Application.UseCases.Tests.Handlers
{
    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IProductMapper> _mapperMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IProductMapper>();
            _handler = new CreateProductCommandHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task HandleShouldCreateProductAndReturnId()
        {
            // Arrange
            var dto = new CreateProductCommandDto { Name = "Test Product", Description = "Test Description", Price = 10.0m };
            var command = new CreateProductCommand(dto);
            var product = new Product { Name = dto.Name, Description = dto.Description, Price = dto.Price };

            _mapperMock.Setup(m => m.MapToProduct(dto)).Returns(product);
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.MapToProduct(dto), Times.Once);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
            Assert.Equal(product.Id, result);
        }

        [Fact]
        public async Task HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
        {
            // Arrange
            CreateProductCommand? command = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
