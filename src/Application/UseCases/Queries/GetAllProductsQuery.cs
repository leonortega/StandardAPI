using MediatR;
using StandardAPI.Application.DTOs;

namespace StandardAPI.Application.UseCases.Queries
{
    public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
    }
}