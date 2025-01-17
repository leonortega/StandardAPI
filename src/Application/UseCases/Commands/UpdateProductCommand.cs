using MediatR;
using StandardAPI.Application.DTOs;

namespace StandardAPI.Application.UseCases.Commands
{
    public class UpdateProductCommand : IRequest<int>
    {
        public required UpdateProductCommandDto Dto { get; set; }
    }
}
