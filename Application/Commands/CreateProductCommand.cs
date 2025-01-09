using MediatR;

namespace StandardAPI.Application.Commands
{
    public record CreateProductCommand(string? Name, decimal Price) : IRequest<Guid>;
}
