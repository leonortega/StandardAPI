using MediatR;
using Microsoft.Extensions.Logging;
using StandardAPI.Application.UseCases.Commands;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Application.UseCases.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, int>
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(IProductRepository repository, ILogger<DeleteProductCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<int> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Deleting product with ID: {ProductId}", request.Id);

            var result = await _repository.DeleteAsync(request.Id);

            _logger.LogInformation("Product with ID: {ProductId} deleted successfully.", request.Id);

            return result;
        }
    }
}