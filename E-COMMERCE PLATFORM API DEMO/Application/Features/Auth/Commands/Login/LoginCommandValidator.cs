using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Thiếu tên đăng nhập");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Thiếu mật khẩu");
                
        }
    }
}
