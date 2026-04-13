using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
namespace Application.Features.Auth.Commands.Register
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator(IUserRepository userRepo)
        {
            RuleFor(x => x.username)
                .NotEmpty().WithMessage("Tên người dùng không nên để trống")
                .MinimumLength(3).WithMessage("Tên người dùng có ít nhất là 3 kí tự");

            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email cũng không được để trống lun nhé")
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage("Định dạng không hợp nha :3")
                .MustAsync(async (email, cancellation) => await userRepo.IsEmailUniqueAsync(email))
                .WithMessage("Email này đã có ròi nho");
            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Mật khẩu mà còn để trống hả?")
                .MinimumLength(6).WithMessage("Tối thiểu là 6 kí tự nho~~");
        }
    }
}
