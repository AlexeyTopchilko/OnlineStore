using CartMicroservice.Service.Services.CartService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartMicroservice.Service.Services.CartService
{
    public interface ICartService
    {
        Task DeleteAsync(Domain.Entities.Cart cart);

        Task<IEnumerable<CartViewModel>> GetAllAsync();

        Task<Domain.Entities.Cart> GetById(int id);

        Task<IEnumerable<Domain.Entities.Cart>> GetAllByUser(Guid id);

        Task AddToCart(AddToCartModel model);

        Task RemoveCartLine(int id);

        Task ChangeQuantity(int id, int quantity);

        Task<CartViewModel> GetActiveCartByUser(Guid userId);

        Task LockCart(int orderId);
    }
}