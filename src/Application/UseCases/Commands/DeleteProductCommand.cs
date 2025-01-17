using MediatR;

namespace StandardAPI.Application.UseCases.Commands
{
    public class DeleteProductCommand : IRequest<int>
    {
        public Guid Id { get; set; }
    }
}
