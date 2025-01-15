using MediatR;
using StandardAPI.Application.DTOs;

namespace StandardAPI.Application.UseCases.Commands
{
    public class CreateProductCommand(CreateProductCommandDto dto) : IRequest<Guid>
    {
        public CreateProductCommandDto Dto { get; } = dto;
    }
}
