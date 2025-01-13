using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using StandardAPI.Application.Mappers;
using StandardAPI.Application.UseCases.Validators;


namespace StandardAPI.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //Mappers
            services.AddSingleton<ProductMapper>();

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();


            return services;
        }
    }
}
