using MediatR;
using Microsoft.AspNetCore.Mvc;
using StandardAPI.Application.DTOs;
using StandardAPI.Application.UseCases.Commands;
using StandardAPI.Application.UseCases.Queries;

namespace StandardAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            return product != null ? Ok(product) : NotFound();
        }
    }
}
