using MediatR;
using Microsoft.Extensions.Logging;
using StandardAPI.Application.Interfaces;
using StandardAPI.Application.UseCases.Commands;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Application.UseCases.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, int>
    {
        private readonly IProductRepository _repository;
        private readonly IProductMapper _mapper;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(IProductRepository repository, IProductMapper mapper, ILogger<UpdateProductCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Updating product with ID: {ProductId}", request.Dto.Id);

            var product = _mapper.MapToProduct(request.Dto);
            var result = await _repository.UpdateAsync(product);

            _logger.LogInformation("Product with ID: {ProductId} updated successfully.", request.Dto.Id);

            return result;
        }
    }
}