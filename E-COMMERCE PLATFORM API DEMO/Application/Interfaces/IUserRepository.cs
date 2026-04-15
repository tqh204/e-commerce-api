using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        public Task AddAsync(User user);
        public Task<bool> IsEmailUniqueAsync(string email);
        Task<User?> GetByEmailAsync(string email);
        Task<Role?> GetRoleByNameAsync(string rolename);
    }
}
