using Application.Features.Auth.Commands.Register;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Domain.Entities;

namespace Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public LoginCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            string email = request.email;
            string password = request.password;
            //Finding and try checking user in DB if it exsist
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return Result<AuthResponse>.Failure("Tài khoản hoặc mật khẩu sai");
            //Compared the present password and the password has saved in DB
            bool passwordHashCompared = BCrypt.Net.BCrypt.Verify(password, user.passwordHash);
            //If true, return an accesstoken
            if(passwordHashCompared)
            {
                //Preparing parts to create Token
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.email.ToString()),
                    new Claim(ClaimTypes.Name, user.username),
                    new Claim(ClaimTypes.Role, user.role.RoleName.ToString())
                };

                var jwtKey = _configuration["Jwt:Key"]!;
                var jwtIssuer = _configuration["Jwt:Issuer"]!;
                var jwtAudience = _configuration["Jwt:Audience"]!;
                //Preparing signature key
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));//SymmetricSecurityKey(Encoding.UTF8.GetBytes used to sign
                //encrypt signature key by Sha256
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//SigningCredentials used to encrypt
                //Package token together
                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: creds
                    );
                //Convert to string 
                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);//JwtSecurityTokenHandler used to convert from Object to string
                var response = new AuthResponse(accessToken, user.role.RoleName);
                return Result<AuthResponse>.Success(response);
            }
            else
            {
                return Result<AuthResponse>.Failure("Tài khoản hoặc mật khẩu sai");
            }
        }
    }
}
