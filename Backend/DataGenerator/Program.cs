using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Model;
using System.IO;

namespace DataGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddDbContext<WebstoreContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetService<WebstoreContext>())
            {
                SeedData(context);
            }

            Console.WriteLine("Data seeding completed successfully.");
        }

        private static void SeedData(WebstoreContext context)
        {
            
            context.BasketPositions.RemoveRange(context.BasketPositions);
            context.OrderPositions.RemoveRange(context.OrderPositions);
            context.Orders.RemoveRange(context.Orders);
            context.Products.RemoveRange(context.Products);
            context.ProductGroups.RemoveRange(context.ProductGroups);
            context.Users.RemoveRange(context.Users);
            context.UserGroups.RemoveRange(context.UserGroups);
            context.SaveChanges();
            Console.WriteLine("Cleared existing data.");

            
            var userGroups = new List<UserGroup>
            {
                new UserGroup { Name = "Admin Group" },
                new UserGroup { Name = "Customer Group" }
            };
            context.UserGroups.AddRange(userGroups);
            context.SaveChanges();
            Console.WriteLine("Added UserGroups.");

            
            var users = new List<User>
            {
                new User
                {
                    Login = "admin",
                    Password = "password",
                    Type = UserType.Admin,
                    IsActive = true,
                    GroupID = userGroups[0].ID
                },
                new User
                {
                    Login = "customer1",
                    Password = "password",
                    Type = UserType.Casual,
                    IsActive = true,
                    GroupID = userGroups[1].ID
                },
                new User
                {
                    Login = "customer2",
                    Password = "password",
                    Type = UserType.Casual,
                    IsActive = true,
                    GroupID = userGroups[1].ID
                }
            };
            context.Users.AddRange(users);
            context.SaveChanges();
            Console.WriteLine("Added Users.");

            
            var parentProductGroups = new List<ProductGroup>
            {
                new ProductGroup { Name = "Electronics" },
                new ProductGroup { Name = "Books" },
                new ProductGroup { Name = "Clothing" }
            };
            context.ProductGroups.AddRange(parentProductGroups);
            context.SaveChanges();
            Console.WriteLine("Added parent ProductGroups.");

                    
             var childProductGroups = new List<ProductGroup>
            {
                new ProductGroup { Name = "Laptops", ParentID = parentProductGroups[0].ID },
                new ProductGroup { Name = "Smartphones", ParentID = parentProductGroups[0].ID },
                new ProductGroup { Name = "Fiction", ParentID = parentProductGroups[1].ID },
                new ProductGroup { Name = "Non-Fiction", ParentID = parentProductGroups[1].ID },
                new ProductGroup { Name = "Men's Clothing", ParentID = parentProductGroups[2].ID },
                new ProductGroup { Name = "Women's Clothing", ParentID = parentProductGroups[2].ID }
            };
            context.ProductGroups.AddRange(childProductGroups);
            context.SaveChanges();
            Console.WriteLine("Added child ProductGroups.");

            
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Laptop",
                    Price = 999.99,
                    Image = "laptop.png",
                    IsActive = true,
                    GroupID = childProductGroups[0].ID
                },
                new Product
                {
                    Name = "Smartphone",
                    Price = 499.99,
                    Image = "smartphone.png",
                    IsActive = true,
                    GroupID = childProductGroups[1].ID
                },
                new Product
                {
                    Name = "Novel",
                    Price = 19.99,
                    Image = "novel.png",
                    IsActive = true,
                    GroupID = childProductGroups[2].ID
                },
                new Product
                {
                    Name = "T-Shirt",
                    Price = 9.99,
                    Image = "tshirt.png",
                    IsActive = true,
                    GroupID = childProductGroups[4].ID
                },
                new Product
                {
                    Name = "Tablet",
                    Price = 299.99,
                    Image = "tablet.png",
                    IsActive = true,
                    GroupID = childProductGroups[1].ID
                },
                new Product
                {
                    Name = "E-Reader",
                    Price = 129.99,
                    Image = "ereader.png",
                    IsActive = true,
                    GroupID = childProductGroups[3].ID
                },
                new Product
                {
                    Name = "Jeans",
                    Price = 49.99,
                    Image = "jeans.png",
                    IsActive = true,
                    GroupID = childProductGroups[4].ID
                }
            };
            context.Products.AddRange(products);
            context.SaveChanges();
            Console.WriteLine("Added Products.");

            
            var orders = new List<Order>
            {
                new Order
                {
                    UserID = users[1].ID,
                    Date = DateTime.Now,
                    IsPaid = false
                },
                new Order
                {
                    UserID = users[2].ID,
                    Date = DateTime.Now,
                    IsPaid = true
                }
            };
            context.Orders.AddRange(orders);
            context.SaveChanges();
            Console.WriteLine("Added Orders.");

            
            var orderPositions = new List<OrderPosition>
            {
                new OrderPosition
                {
                    OrderID = orders[0].ID,
                    ProductID = products[0].ID,
                    Amount = 1,
                    Price = products[0].Price
                },
                new OrderPosition
                {
                    OrderID = orders[0].ID,
                    ProductID = products[2].ID,
                    Amount = 2,
                    Price = products[2].Price
                },
                new OrderPosition
                {
                    OrderID = orders[1].ID,
                    ProductID = products[1].ID,
                    Amount = 1,
                    Price = products[1].Price
                },
                new OrderPosition
                {
                    OrderID = orders[1].ID,
                    ProductID = products[3].ID,
                    Amount = 3,
                    Price = products[3].Price
                },
                new OrderPosition
                {
                    OrderID = orders[1].ID,
                    ProductID = products[4].ID,
                    Amount = 1,
                    Price = products[4].Price
                }
            };
            context.OrderPositions.AddRange(orderPositions);
            context.SaveChanges();
            Console.WriteLine("Added OrderPositions.");

            
            var basketPositions = new List<BasketPosition>
            {
                new BasketPosition
                {
                    ProductID = products[0].ID,
                    UserID = users[1].ID,
                    Amount = 1
                },
                new BasketPosition
                {
                    ProductID = products[1].ID,
                    UserID = users[1].ID,
                    Amount = 2
                },
                new BasketPosition
                {
                    ProductID = products[2].ID,
                    UserID = users[2].ID,
                    Amount = 1
                },
                new BasketPosition
                {
                    ProductID = products[3].ID,
                    UserID = users[2].ID,
                    Amount = 3
                },
                new BasketPosition
                {
                    ProductID = products[4].ID,
                    UserID = users[1].ID,
                    Amount = 1
                },
                new BasketPosition
                {
                    ProductID = products[5].ID,
                    UserID = users[2].ID,
                    Amount = 2
                }
            };
            context.BasketPositions.AddRange(basketPositions);
            context.SaveChanges();
            Console.WriteLine("Added BasketPositions.");
        }
    }
}