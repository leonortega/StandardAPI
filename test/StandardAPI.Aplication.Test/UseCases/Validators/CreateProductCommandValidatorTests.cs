using FluentValidation.TestHelper;
using StandardAPI.Application.DTOs;
using StandardAPI.Application.UseCases.Validators;
using Xunit;

namespace StandardAPI.Application.Tests.UseCases.Validators
{
    public class CreateProductCommandValidatorTests
    {
        private readonly CreateProductCommandValidator _validator;

        public CreateProductCommandValidatorTests()
        {
            _validator = new CreateProductCommandValidator();
        }

        [Fact]
        public void ShouldHaveErrorWhenNameIsEmpty()
        {
            var model = new CreateProductCommandDto { Name = string.Empty, Price = 10 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Product name is required.");
        }

        [Fact]
        public void ShouldNotHaveErrorWhenNameIsNotEmpty()
        {
            var model = new CreateProductCommandDto { Name = "Valid Name", Price = 10 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void ShouldHaveErrorWhenPriceIsZero()
        {
            var model = new CreateProductCommandDto { Name = "Valid Name", Price = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Price).WithErrorMessage("Price must be greater than zero.");
        }

        [Fact]
        public void ShouldHaveErrorWhenPriceIsNegative()
        {
            var model = new CreateProductCommandDto { Name = "Valid Name", Price = -1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Price).WithErrorMessage("Price must be greater than zero.");
        }

        [Fact]
        public void ShouldNotHaveErrorWhenPriceIsGreaterThanZero()
        {
            var model = new CreateProductCommandDto { Name = "Valid Name", Price = 10 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Price);
        }
    }
}