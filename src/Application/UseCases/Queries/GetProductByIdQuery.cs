using MediatR;
using StandardAPI.Application.DTOs;

namespace StandardAPI.Application.UseCases.Queries
{
    public class GetProductByIdQuery(Guid id) : IRequest<ProductDto>
    {
        public Guid Id { get; } = id;
    }
}