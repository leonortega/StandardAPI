using FluentValidation;
using StandardAPI.Application.UseCases.Queries;

namespace StandardAPI.Application.UseCases.Validators
{
    public class GetProductsByPriceRangeQueryValidator : AbstractValidator<GetProductsByPriceRangeQuery>
    {
        public GetProductsByPriceRangeQueryValidator()
        {
            RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).WithMessage("Minimum price must be greater than or equal to zero.");
            RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(x => x.MinPrice).WithMessage("Maximum price must be greater than or equal to minimum price.");
        }
    }
}
