using System;
using System.Collections.Generic;
using System.Text;
using BCrypt;
using MediatR;
using Domain.Entities;
using Domain.Interfaces;
namespace Application.Features.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            //Checking unique
            var isUnique = await _userRepository.IsEmailUniqueAsync(request.email);
            if (!isUnique)
                throw new Exception("Email đã tồn tại");
            //Encrypting by BCrypt
            var userRole = await _userRepository.GetRoleByNameAsync("User");
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.password);


            //Creating new user
            var user = new User
            {
                Id = Guid.NewGuid(),
                username = request.username,
                email = request.email,
                passwordHash = passwordHash,
                roleId = userRole.roleId,
                createAt = DateTime.UtcNow,
            };
            //Throwing to DbContext
            await _userRepository.AddAsync(user);
            //Saving to DB
            await _unitOfWork.SaveChangesAsync();
            return user.Id;
        }
    }
}
