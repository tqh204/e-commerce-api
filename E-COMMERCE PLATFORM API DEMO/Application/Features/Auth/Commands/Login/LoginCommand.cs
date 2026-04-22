using Application.Common.Results;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string email, string password) : IRequest<Result<AuthResponse>>;
}
