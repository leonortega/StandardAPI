using MediatR;
using Microsoft.Extensions.Logging;
using StandardAPI.Application.DTOs;
using StandardAPI.Application.Interfaces;
using StandardAPI.Application.UseCases.Queries;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Application.UseCases.Handlers
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _repository;
        private readonly IProductMapper _mapper;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(IProductRepository repository, IProductMapper mapper, ILogger<GetAllProductsQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving all products.");

            var products = await _repository.GetAllAsync();
            var productDtos = products.Select(product => _mapper.MapToProductDto(product));

            _logger.LogInformation("All products retrieved successfully.");

            return productDtos;
        }
    }
}
