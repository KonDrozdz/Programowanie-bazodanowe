
using BLL.ServiceInterfaces;
using BLLDB.Services;
//using BLLDB_EF.Services;
using DAL;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace WebApi
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<WebstoreContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            //builder.Services.AddScoped<IProductService, ProductService>();
            //builder.Services.AddScoped<IUserService, UserService>();
            //builder.Services.AddScoped<IOrderService, OrderService>();
            //builder.Services.AddScoped<IProductGroupService, ProductGroupService>();
            //builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
            builder.Services.AddScoped<IProductService>(provider => new ProductService(connectionString));
            builder.Services.AddScoped<IUserService>(provider => new UserService(connectionString));
            builder.Services.AddScoped<IOrderService>(provider => new OrderService(connectionString));
            builder.Services.AddScoped<IProductGroupService>(provider => new ProductGroupService(connectionString));
            builder.Services.AddScoped<IShoppingCartService>(provider => new ShoppingCartService(connectionString));



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
