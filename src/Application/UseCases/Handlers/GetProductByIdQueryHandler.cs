using MediatR;
using Microsoft.Extensions.Logging;
using StandardAPI.Application.DTOs;
using StandardAPI.Application.Mappers;
using StandardAPI.Application.UseCases.Queries;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Application.UseCases.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _repository;
        private readonly ProductMapper _mapper;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(
            IProductRepository repository,
            ProductMapper mapper,
            ILogger<GetProductByIdQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Retrieving product with ID: {ProductId}", request.Id);

            var product = await _repository.GetByIdAsync(request.Id);

            if (product == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found.", request.Id);
                return null;
            }

            _logger.LogInformation("Product with ID: {ProductId} retrieved successfully.", request.Id);

            return _mapper.MapToProductDto(product);
        }
    }
}
