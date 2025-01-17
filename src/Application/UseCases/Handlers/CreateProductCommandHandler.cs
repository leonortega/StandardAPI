using MediatR;
using Microsoft.Extensions.Logging;
using StandardAPI.Application.Interfaces;
using StandardAPI.Application.UseCases.Commands;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Application.UseCases.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _repository;
        private readonly IProductMapper _mapper;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(IProductRepository repository, IProductMapper mapper, ILogger<CreateProductCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateProductCommand? request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Creating a new product.");

            var product = _mapper.MapToProduct(request.Dto);
            product.Id = Guid.NewGuid();

            await _repository.AddAsync(product);

            _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

            return product.Id;
        }
    }
}
