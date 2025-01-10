using MediatR;

namespace StandardAPI.Application.UseCases.Commands
{
    public record CreateProductCommand(string? Name, decimal Price) : IRequest<Guid>;
}
