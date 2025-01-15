using MediatR;
using StandardAPI.Application.DTOs;
using StandardAPI.Application.Mappers;
using StandardAPI.Application.UseCases.Commands;
using StandardAPI.Domain.Interfaces;

namespace StandardAPI.Application.UseCases.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _repository;
        private readonly ProductMapper _mapper;

        public CreateProductCommandHandler(IProductRepository repository, ProductMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var product = _mapper.MapToProduct(request.Dto);

            product.Id = Guid.NewGuid();

            await _repository.AddAsync(product);

            return product.Id;
        }
    }
}
