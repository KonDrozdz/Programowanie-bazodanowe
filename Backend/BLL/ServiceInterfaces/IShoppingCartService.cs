using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ServiceInterfaces
{
    public interface IShoppingCartService
    {

        Task AddProductToBasketAsync(int productId, int userId, int amount);
        Task UpdateProductQuantityInBasketAsync(int productId, int userId, int newAmount);
        Task RemoveProductFromBasketAsync(int productId, int userId);
    }
}
