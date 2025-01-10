using MediatR;
using StandardAPI.Domain.Entities;

namespace StandardAPI.Application.UseCases.Queries
{
    public record GetProductByIdQuery(Guid Id) : IRequest<Product>;
}