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
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Review> Reviews { get; set;  }
        public DbSet<Variant> Variants { get; set; }
        public DbSet<PromotionRule> PromotionRules { get; set; }
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
                entity.Property(u => u.rank)
                      .HasConversion<string>()
                      .IsRequired()
                      .HasMaxLength(20);

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

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.cartId);

                entity.Property(c => c.createdAt)
                      .IsRequired();

                entity.Property(c => c.updatedAt)
                      .IsRequired(false);

                entity.HasIndex(c => c.userId).IsUnique();

                entity.HasOne(c => c.user)
                      .WithOne(u => u.cart)
                      .HasForeignKey<Cart>(c => c.userId);

                entity.HasMany(c => c.items)
                      .WithOne(i => i.cart)
                      .HasForeignKey(i => i.cartId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(i => i.discountAmount)
                      .HasColumnType("decimal(18,2)")
                      .HasDefaultValue(0);

                entity.Property(i => i.couponCode)
                      .HasMaxLength(100)
                      .IsRequired(false);

                entity.HasOne(c => c.coupon)
                      .WithMany()
                      .HasForeignKey(u => u.couponId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(i => i.cartItemId);

                entity.Property(i => i.quantity)
                      .IsRequired();

                entity.Property(i => i.unitPrice)
                      .HasColumnType("decimal(18,2)");

                entity.Property(i => i.createdAt)
                      .IsRequired();

                entity.Property(i => i.updatedAt)
                      .IsRequired(false);

                entity.HasIndex(i => new { i.cartId, i.productId })
                      .IsUnique();

                entity.HasOne(i => i.product)
                      .WithMany()
                      .HasForeignKey(i => i.productId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.orderId);

                entity.Property(o => o.totalAmount)
                      .HasColumnType("decimal(18,2)");

                entity.Property(o => o.status)
                      .HasConversion<string>()
                      .IsRequired()
                      .HasMaxLength(50);


                entity.HasOne(o => o.user)
                      .WithMany()
                      .HasForeignKey(o => o.userId);

                entity.HasMany(o => o.items)
                      .WithOne(i => i.order)
                      .HasForeignKey(i => i.orderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(i => i.orderItemId);

                entity.Property(i => i.productName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(i => i.unitPrice)
                      .HasColumnType("decimal(18,2)");

                entity.Property(i => i.lineTotal)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(i => i.product)
                      .WithMany()
                      .HasForeignKey(i => i.productId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(c => c.couponId);

                entity.Property(c => c.code)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.HasIndex(c => c.code)
                      .IsUnique();

                entity.Property(c => c.discountType)
                      .HasConversion<string>()
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(c => c.value)
                      .HasColumnType("decimal(18,2)");

                entity.Property(c => c.minOrderValue)
                      .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.reviewId);
                entity.Property(r => r.rating)
                      .IsRequired();
                entity.Property(r => r.comment)
                      .HasMaxLength(1000)
                      .IsRequired(false);
                entity.HasIndex(r => new { r.userId, r.productId })
                      .IsUnique();
                entity.HasOne(r => r.user)
                      .WithMany()
                      .HasForeignKey(r => r.userId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.product)
                      .WithMany()
                      .HasForeignKey(r => r.productId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Variant>(entity =>
            {
                entity.HasKey(v => v.variantId);

                entity.Property(v => v.sku)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(v => v.sku).IsUnique().HasFilter("[isDeleted] = 0");

                entity.Property(v => v.price)
                      .HasColumnType("decimal(18,2)");

                entity.Property(v => v.inventory)
                      .IsRequired();

                entity.Property(v => v.isDeleted)
                      .HasDefaultValue(false);

                entity.HasOne(v => v.product)
                      .WithMany(p => p.variants)
                      .HasForeignKey(v => v.productId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PromotionRule>(entity =>
            {
                entity.HasKey(r => r.ruleId);

                entity.Property(r => r.ruleName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(r => r.ruleType)
                      .HasConversion<string>()
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(r => r.priority)
                      .IsRequired();

                entity.Property(r => r.isActive)
                      .HasDefaultValue(true);

                entity.Property(r => r.isDeleted)
                      .HasDefaultValue(false);

                entity.Property(r => r.discountType)
                      .HasConversion<string>()
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(r => r.value)
                      .HasColumnType("decimal(18,2)");

                entity.Property(r => r.maxDiscountAmount)
                      .HasColumnType("decimal(18,2)");

                entity.Property(r => r.minOrderValue)
                      .HasColumnType("decimal(18,2)");

                entity.HasIndex(r => new { r.isActive, r.isDeleted, r.startDate, r.endDate, r.priority });
            });

        }
    }
}
