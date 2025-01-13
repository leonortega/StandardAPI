namespace StandardAPI.Application.DTOs
{
    public class CreateProductCommandDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
