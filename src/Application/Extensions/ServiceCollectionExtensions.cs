﻿using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using StandardAPI.Application.Interfaces;
using StandardAPI.Application.Mappers;
using StandardAPI.Application.UseCases.Commands;
using StandardAPI.Application.UseCases.Queries;
using StandardAPI.Application.UseCases.Validators;


namespace StandardAPI.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //Mappers
            services.AddSingleton<ProductMapper>();
            services.AddSingleton<IProductMapper, ProductMapper>();

            //Validations
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<DeleteProductCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<GetProductByIdQueryValidator>();
            services.AddValidatorsFromAssemblyContaining<GetProductsByPriceRangeQueryValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateProductCommandValidator>();

            // MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DeleteProductCommand).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateProductCommand).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllProductsQuery).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProductByIdQuery).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProductsByPriceRangeQuery).Assembly));

            return services;
        }
    }
}
