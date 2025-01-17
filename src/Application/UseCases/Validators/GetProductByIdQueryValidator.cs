using FluentValidation;
using StandardAPI.Application.UseCases.Queries;

namespace StandardAPI.Application.UseCases.Validators
{
    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID is required.");
        }
    }
}
