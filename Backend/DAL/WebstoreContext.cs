using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Model;

namespace DAL
{
    public class WebstoreContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<BasketPosition> BasketPositions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderPosition> OrderPositions { get; set; }

        public WebstoreContext(DbContextOptions<WebstoreContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BasketPosition>().HasKey(bp => new { bp.ProductID, bp.UserID });
            modelBuilder.Entity<OrderPosition>().HasKey(op => new { op.OrderID, op.ProductID });

            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductGroup)
                .WithMany(pg => pg.Products)
                .HasForeignKey(p => p.GroupID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProductGroup>()
                .HasOne(pg => pg.ParentGroup)
                .WithMany(pg =>pg.Children)
                .HasForeignKey(pg => pg.ParentID)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserGroup)
                .WithMany(ug=> ug.Users)
                .HasForeignKey(u => u.GroupID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BasketPosition>()
                .HasOne(bp => bp.Product)
                .WithMany(p=>p.BasketPositions)
                .HasForeignKey(bp => bp.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BasketPosition>()
                .HasOne(bp => bp.User)
                .WithMany(u=>u.BasketPosition)
                .HasForeignKey(bp => bp.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u=>u.Orders)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderPosition>()
                .HasOne(op => op.Order)
                .WithMany(o=>o.OrderPositions)
                .HasForeignKey(op => op.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderPosition>()
                .HasOne(op => op.Product)
                .WithMany(o=>o.OrderPositions)
                .HasForeignKey(op => op.ProductID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
