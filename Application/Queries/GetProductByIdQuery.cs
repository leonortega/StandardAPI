﻿using MediatR;
using StandardAPI.Domain.Entities;

namespace StandardAPI.Application.Queries
{
    public record GetProductByIdQuery(Guid Id) : IRequest<Product>;
}