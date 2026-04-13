using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.email).IsUnique();
                entity.Property(u => u.username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.email).IsRequired().HasMaxLength(130);
                entity.Property(u => u.passwordHash).IsRequired();
                entity.HasOne(u => u.role).WithMany().HasForeignKey(u => u.roleId);
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(u => u.roleId);
                entity.Property(u => u.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(u => u.RoleName).IsUnique();
            });
        }

    }
}
