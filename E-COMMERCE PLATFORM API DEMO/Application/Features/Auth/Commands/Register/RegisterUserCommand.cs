using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Domain.Entities;
namespace Application.Features.Auth.Commands.Register
{
    public record RegisterUserCommand
    (
        string username,
        string email,
        string password
    ) : IRequest<Guid>;//User instead
}
