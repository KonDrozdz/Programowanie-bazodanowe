using BLL.ServiceInterfaces;
using DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Model;

namespace BLL_EF.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly WebstoreContext _context;

        public ShoppingCartService(WebstoreContext context)
        {
            _context = context;
        }


        public async Task AddProductToBasketAsync(int productId, int userId, int amount)
        {

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ID == productId && p.IsActive);
            if (product == null)
                throw new ArgumentException("Product not found or inactive");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null)
                throw new ArgumentException("User not found");


            var existingBasketPosition = await _context.BasketPositions
                .Where(bp => bp.ProductID == productId && bp.UserID == userId)
                .FirstOrDefaultAsync();

            if (existingBasketPosition != null)
            {

                existingBasketPosition.Amount += amount;
            }
            else
            {

                var basketPosition = new BasketPosition
                {
                    ProductID = productId,
                    UserID = userId,
                    Amount = amount
                };
                _context.BasketPositions.Add(basketPosition);
            }

            await _context.SaveChangesAsync();
        }

 
        public async Task UpdateProductQuantityInBasketAsync(int productId, int userId, int newAmount)
        {

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ID == productId && p.IsActive);
            if (product == null)
                throw new ArgumentException("Product not found or inactive");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null)
                throw new ArgumentException("User not found");

           
            var basketPosition = await _context.BasketPositions
                .Where(bp => bp.ProductID == productId && bp.UserID == userId)
                .FirstOrDefaultAsync();

            if (basketPosition == null)
                throw new ArgumentException("Product not found in basket");

            basketPosition.Amount = newAmount;

            await _context.SaveChangesAsync();
        }


        public async Task RemoveProductFromBasketAsync(int productId, int userId)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID== userId);
            if (user == null)
                throw new ArgumentException("User not found");


            var basketPosition = await _context.BasketPositions
                .Where(bp => bp.ProductID == productId && bp.UserID == userId)
                .FirstOrDefaultAsync();

            if (basketPosition == null)
                throw new ArgumentException("Product not found in basket");

            _context.BasketPositions.Remove(basketPosition);
            await _context.SaveChangesAsync();
        }
    }
}
