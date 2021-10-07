using CartMicroservice.Service.Services.CartService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CartMicroservice.Domain.Entities;

namespace CartMicroservice.Service.Services.CartService
{
    public interface ICartService
    {
        Task DeleteAsync(Cart cart);

        Task<IEnumerable<CartViewModel>> GetAllAsync();

        Task<Cart> GetById(int id);

        Task<IEnumerable<Cart>> GetAllByUser(Guid id);

        Task AddToCart(AddToCartModel model);

        //Task RemoveCartLine(int id);

        //Task ChangeQuantity(int id, int quantity);

        Task<CartViewModel> GetActiveCartByUser(Guid userId);

        Task LockTheCart(int orderId);

        Task<int> GetTotalQuantity(Guid userId);

        Task UpdateCart(CartViewModel model);
    }
}