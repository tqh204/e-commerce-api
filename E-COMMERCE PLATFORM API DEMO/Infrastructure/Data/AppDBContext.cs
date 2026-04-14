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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
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

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(u => u.roleId);
                entity.Property(u => u.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(u => u.RoleName).IsUnique();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(u => u.productId);
                entity.Property(u => u.productName).IsRequired().HasMaxLength(200);
                entity.Property(u => u.price).HasColumnType("decimal(18,2)");
                entity.Property(u => u.stockQuantity).IsRequired();
                entity.Property(u => u.isDeleted).HasDefaultValue(false);
                entity.HasOne(u => u.category).WithMany(c => c.Products).HasForeignKey(u => u.categoryId);

            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(u => u.categoryId);
                entity.Property(u => u.categoryName).IsRequired().HasMaxLength(200);
                entity.HasIndex(u => u.categoryName).IsUnique();
            });

        }
    }
}
