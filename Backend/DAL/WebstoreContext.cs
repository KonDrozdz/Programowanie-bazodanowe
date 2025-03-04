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
                .HasForeignKey(p => p.GroupID);

            modelBuilder.Entity<ProductGroup>()
                .HasOne(pg => pg.ParentGroup)
                .WithMany()
                .HasForeignKey(pg => pg.ParentID);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserGroup)
                .WithMany()
                .HasForeignKey(u => u.GroupID);

            modelBuilder.Entity<BasketPosition>()
                .HasOne(bp => bp.Product)
                .WithMany()
                .HasForeignKey(bp => bp.ProductID);

            modelBuilder.Entity<BasketPosition>()
                .HasOne(bp => bp.User)
                .WithMany()
                .HasForeignKey(bp => bp.UserID);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserID);

            modelBuilder.Entity<OrderPosition>()
                .HasOne(op => op.Order)
                .WithMany()
                .HasForeignKey(op => op.OrderID);

            modelBuilder.Entity<OrderPosition>()
                .HasOne(op => op.Product)
                .WithMany()
                .HasForeignKey(op => op.ProductID);
        }
    }
}
