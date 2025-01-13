using FluentValidation;
using StandardAPI.Application.DTOs;

namespace StandardAPI.Application.UseCases.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommandDto>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        }
    }
}
