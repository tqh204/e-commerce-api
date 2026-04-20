using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _context;

        public UserRepository(AppDBContext context) => _context = context;

        public async Task AddAsync(User user) => await _context.Users.AddAsync(user);//Add entity to DbContext to prepare for insert

        public async Task<bool> IsEmailUniqueAsync(string email) => !await _context.Users.AnyAsync(u => u.email == email);// Check user email exsits?

        public async Task<User?> GetByEmailAsync(string email) => await _context.Users.Include(u => u.role).FirstOrDefaultAsync(u => u.email == email); //Find user satisfied conditions, if not, return null

        public async Task<Role?> GetRoleByNameAsync(string roleName) => await _context.Set<Role>().FirstOrDefaultAsync(u => u.RoleName == roleName);//Find nameRole

        public async Task<User?> GetUserByIdAsync(Guid userId) => await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        public void Update(User user) => _context.Users.Update(user);
    }
}
