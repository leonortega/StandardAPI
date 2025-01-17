using FluentValidation;
using StandardAPI.Application.UseCases.Commands;

namespace StandardAPI.Application.UseCases.Validators
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID is required.");
        }
    }
}
