using Microsoft.EntityFrameworkCore;
using PosStore.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PosStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            // Sale configuration
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // SaleItem configuration
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);

                entity.HasOne(e => e.Sale)
                    .WithMany(s => s.SaleItems)
                    .HasForeignKey(e => e.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.SaleItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.FullName).HasComputedColumnSql("concat([FirstName],' ',[LastName])", stored: true);
                entity.Property(e => e.Initials).HasComputedColumnSql("concat(left([FirstName],(1)),left([LastName],(1)))", stored: true);
            });

            // Project configuration
            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Budget).HasPrecision(15, 2);
                entity.Property(e => e.ActualCost).HasPrecision(15, 2);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Projects)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data based on productlistdata.js
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Products (based on your productlistdata.js)
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Lenovo 3rd Generation",
                    SKU = "PT001",
                    Category = "Laptop",
                    Brand = "Lenovo",
                    Price = 12500.00m,
                    Quantity = 100,
                    ImagePath = "assets/img/products/stock-img-01.png",
                    CreatedBy = "Arroon",
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "Bold V3.2",
                    SKU = "PT002",
                    Category = "Electronics",
                    Brand = "Bolt",
                    Price = 1600.00m,
                    Quantity = 140,
                    ImagePath = "assets/img/products/stock-img-06.png",
                    CreatedBy = "Kenneth",
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Id = 3,
                    Name = "Nike Jordan",
                    SKU = "PT003",
                    Category = "Shoe",
                    Brand = "Nike",
                    Price = 6000.00m,
                    Quantity = 780,
                    ImagePath = "assets/img/products/stock-img-02.png",
                    CreatedBy = "Gooch",
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Id = 4,
                    Name = "Apple Series 5 Watch",
                    SKU = "PT004",
                    Category = "Electronics",
                    Brand = "Apple",
                    Price = 25000.00m,
                    Quantity = 450,
                    ImagePath = "assets/img/products/stock-img-03.png",
                    CreatedBy = "Nathan",
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Id = 5,
                    Name = "Amazon Echo Dot",
                    SKU = "PT005",
                    Category = "Speaker",
                    Brand = "Amazon",
                    Price = 1600.00m,
                    Quantity = 477,
                    ImagePath = "assets/img/products/stock-img-04.png",
                    CreatedBy = "Alice",
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    Id = 6,
                    Name = "Lobar Handy",
                    SKU = "PT006",
                    Category = "Furnitures",
                    Brand = "Woodmart",
                    Price = 4521.00m,
                    Quantity = 145,
                    ImagePath = "assets/img/products/stock-img-05.png",
                    CreatedBy = "Robb",
                    CreatedDate = DateTime.UtcNow
                }
            );

            // Seed default admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@dreamspos.com",
                    PasswordHash = "$2a$11$8GvzHzKdXqKf8.nQQGvQKOqZvJZJZqJZJZJZJZJZJZJZJZJZJZJZJZ", // "admin123"
                    FirstName = "System",
                    LastName = "Administrator",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
