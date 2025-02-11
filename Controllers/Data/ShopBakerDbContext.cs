using System;
using ShopBaker.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShopBaker.Controllers.Data
{	
        public class ShopBakerDbContext : DbContext
        {
            public ShopBakerDbContext(DbContextOptions<ShopBakerDbContext> options) : base(options) { }

            public DbSet<CakeModel> Cakes { get; set; }
            public DbSet<BillingDetailsModel> BillingDetails { get; set; }
            public DbSet<CartModel> Carts { get; set; }
            public DbSet<UserModel> Users { get; set; }
            public DbSet<TypeCakeModel> TypeCake { get; set; }
            public DbSet<ChatModel> ChatModels { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Đặt Id làm khóa chính cho bảng TypeCake
            modelBuilder.Entity<TypeCakeModel>().HasKey(t => t.TypeCakeId);

            // Các cấu hình khác (nếu có) cho các bảng khác
        }

    }

    
}

