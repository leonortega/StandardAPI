using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StandardAPI.Application.DTOs;
using StandardAPI.Application.UseCases.Commands;
using StandardAPI.Application.UseCases.Queries;

namespace StandardAPI.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommandDto dto)
        {
            var command = new CreateProductCommand(dto);
            var productId = await _mediator.Send(command);
            return Ok(productId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommandDto dto)
        {
            if (id != dto?.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            var command = new UpdateProductCommand { Dto = dto };
            var result = await _mediator.Send(command);
            return result > 0 ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await _mediator.Send(command);
            return result > 0 ? Ok() : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var query = new GetProductByIdQuery(id);
            var product = await _mediator.Send(query);
            return product != null ? Ok(product) : NotFound();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var query = new GetAllProductsQuery();
            var products = await _mediator.Send(query);

            if (products.Any())
            {
                return Ok(products); 
            }

            return NoContent();
        }

        [HttpGet("price-range")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var query = new GetProductsByPriceRangeQuery { MinPrice = minPrice, MaxPrice = maxPrice };
            var products = await _mediator.Send(query);

            if (products.Any())
            {
                return Ok(products);
            }

            return NoContent();
        }
    }
}