using MediatR;
using StandardAPI.Application.DTOs;

namespace StandardAPI.Application.UseCases.Queries
{
    public class GetProductsByPriceRangeQuery : IRequest<IEnumerable<ProductDto>>
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}
