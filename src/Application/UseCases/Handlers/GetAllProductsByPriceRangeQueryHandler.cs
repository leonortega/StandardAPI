using MediatR;
using Microsoft.Extensions.Logging;
using StandardAPI.Application.DTOs;
using StandardAPI.Application.Interfaces;
using StandardAPI.Application.UseCases.Queries;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Application.UseCases.Handlers
{
    public class GetProductsByPriceRangeQueryHandler : IRequestHandler<GetProductsByPriceRangeQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _repository;
        private readonly IProductMapper _mapper;
        private readonly ILogger<GetProductsByPriceRangeQueryHandler> _logger;

        public GetProductsByPriceRangeQueryHandler(IProductRepository repository, IProductMapper mapper, ILogger<GetProductsByPriceRangeQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetProductsByPriceRangeQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Retrieving products with price range: {MinPrice} - {MaxPrice}", request.MinPrice, request.MaxPrice);

            var products = await _repository.GetProductsByPriceRangeAsync(request.MinPrice, request.MaxPrice);
            var productDtos = products.Select(product => _mapper.MapToProductDto(product));

            _logger.LogInformation("Products with price range: {MinPrice} - {MaxPrice} retrieved successfully.", request.MinPrice, request.MaxPrice);

            return productDtos;
        }
    }
}
