using Riok.Mapperly.Abstractions;
using StandardAPI.Application.DTOs;
using StandardAPI.Domain.Entities;

namespace StandardAPI.Application.Mappers
{
    [Mapper]
    public partial class ProductMapper
    {
        public partial Product MapToProduct(CreateProductCommandDto dto);

        public partial Product MapToProduct(UpdateProductCommandDto dto);

        public partial ProductDto MapToProductDto(Product product);
    }
}