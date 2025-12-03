using MeeshoWebClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Data
{
    public class MeeshoAppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public MeeshoAppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }  
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductColorMapping> ProductColorMappings { get; set; }
        public DbSet<ProductSizeMapping> ProductSizeMappings { get; set; }
        public DbSet<UserLikedProduct> UserLikedProducts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure row version for concurrency control
            //modelBuilder.Entity<Product>()
            //    .Property(p => p.RowVersion)
            //    .IsRowVersion()
            //    .IsConcurrencyToken();

            // Configure the relationship between CartItem and User
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between CartItem and Product
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Defining the relationship between User and UserLikedProduct (Many-to-Many)
            // A user can like multiple products, and each liked product entry belongs to a specific user.
            modelBuilder.Entity<UserLikedProduct>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.LikedProducts)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Defining the relationship between Product and UserLikedProduct (Many-to-Many)
            // A product can be liked by multiple users, and each liked product entry belongs to a specific product.
            modelBuilder.Entity<UserLikedProduct>()
                .HasOne(ul => ul.Product)
                .WithMany(p => p.LikedByUsers)
                .HasForeignKey(ul => ul.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product-User Relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product-Category Relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductColorMapping Configuration
            modelBuilder.Entity<ProductColorMapping>()
                .HasKey(pc => pc.ProductColorMappingId);

            modelBuilder.Entity<ProductColorMapping>()
                .HasOne<Product>()
                .WithMany(p => p.ProductColorMappings)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductColorMapping>()
                .HasOne<ProductColor>()
                .WithMany(c => c.ProductColorMappings)
                .HasForeignKey(m => m.ColorId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductSizeMapping Configuration
            modelBuilder.Entity<ProductSizeMapping>()
                .HasKey(ps => ps.ProductSizeMappingId);

            modelBuilder.Entity<ProductSizeMapping>()
                .HasOne<Product>()
                .WithMany(p => p.ProductSizeMappings)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductSizeMapping>()
                .HasOne<ProductSize>()
                .WithMany(s => s.ProductSizeMappings)
                .HasForeignKey(ps => ps.SizeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Product Categories
            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { Id = Guid.NewGuid(), Name = "Electronics" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Fashion" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Home & Kitchen" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Beauty & Personal Care" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Toys & Games" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Books" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Sports & Outdoors" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Automotive" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Jewelry & Accessories" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Health & Wellness" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Mobile Phones & Accessories" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Office Supplies" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Pet Supplies" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Furniture" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Grocery & Gourmet Foods" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Baby Products" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Music & Instruments" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Watches" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Handmade & Artisanal" },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Industrial & Scientific" }
            );

            // Seed Product Colors
            modelBuilder.Entity<ProductColor>().HasData(
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "Red", ColorHex = "#FF0000" },
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "Blue", ColorHex = "#0000FF" },
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "Green", ColorHex = "#008000" },
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "Yellow", ColorHex = "#FFFF00" },
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "Black", ColorHex = "#000000" },
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "White", ColorHex = "#FFFFFF" },
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "Gray", ColorHex = "#808080" },
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "Purple", ColorHex = "#800080" },
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "Pink", ColorHex = "#FFC0CB" },
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "Orange", ColorHex = "#FFA500" },
                new ProductColor { ColorId = Guid.NewGuid(), ColorName = "Brown", ColorHex = "#A52A2A" }
            );

            // Seed Product Sizes
            modelBuilder.Entity<ProductSize>().HasData(
                new ProductSize { SizeId = Guid.NewGuid(), Size = "XS" },
                new ProductSize { SizeId = Guid.NewGuid(), Size = "S" },
                new ProductSize { SizeId = Guid.NewGuid(), Size = "M" },
                new ProductSize { SizeId = Guid.NewGuid(), Size = "L" },
                new ProductSize { SizeId = Guid.NewGuid(), Size = "XL" },
                new ProductSize { SizeId = Guid.NewGuid(), Size = "XXL" },
                new ProductSize { SizeId = Guid.NewGuid(), Size = "3XL" },
                new ProductSize { SizeId = Guid.NewGuid(), Size = "4XL" },
                new ProductSize { SizeId = Guid.NewGuid(), Size = "5XL" },
                new ProductSize { SizeId = Guid.NewGuid(), Size = "6XL" },
                new ProductSize { SizeId = Guid.NewGuid(), Size = "Free Size" }
            );
        }
    }
}
