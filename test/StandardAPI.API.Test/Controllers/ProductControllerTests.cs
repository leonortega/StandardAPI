using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StandardAPI.API.Controllers;
using StandardAPI.Application.DTOs;
using StandardAPI.Application.UseCases.Commands;
using StandardAPI.Application.UseCases.Queries;

namespace StandardAPI.API.Test.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ProductController(_mediatorMock.Object);
        }

        [Fact]
        public async Task CreateShouldReturnOkWithProductId()
        {
            // Arrange
            var dto = new CreateProductCommandDto { Name = "Test Product", Description = "Test Description", Price = 10.0m };
            var productId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(productId);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(productId, okResult.Value);
        }

        [Fact]
        public async Task GetByIdShouldReturnOkWithProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new ProductDto { Id = productId, Name = "Test Product", Description = "Test Description", Price = 10.0m };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((ProductDto?)product);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(productId, returnedProduct.Id);
        }

        [Fact]
        public async Task GetByIdShouldReturnNotFoundWhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((ProductDto?)null);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
