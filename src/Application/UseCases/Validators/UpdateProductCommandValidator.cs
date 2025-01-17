using FluentValidation;
using StandardAPI.Application.DTOs;

namespace StandardAPI.Application.UseCases.Validators
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommandDto>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        }
    }
}
