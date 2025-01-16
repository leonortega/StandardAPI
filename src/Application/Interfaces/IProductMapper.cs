using StandardAPI.Application.DTOs;
using StandardAPI.Domain.Entities;

namespace StandardAPI.Application.Interfaces
{
    public interface IProductMapper
    {
        Product MapToProduct(CreateProductCommandDto dto);
        Product MapToProduct(UpdateProductCommandDto dto);
        ProductDto MapToProductDto(Product product);
    }
}
