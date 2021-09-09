using AutoMapper;
using CartMicroservice.Domain.Entities;
using CartMicroservice.Repository.Repository;
using CartMicroservice.Service.Services.CartService.Models;
using CartMicroservice.Service.Services.RabbitMqService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartMicroservice.Service.Services.RabbitMqService;

namespace CartMicroservice.Service.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly IRepositoryGeneric<Cart> _cartRepository;

        private readonly IMapper _mapper;

        private readonly IRepositoryGeneric<CartProducts> _cartProductsRepository;

        private readonly IRabbitMqService _rabbitMqService;

        public CartService(IRepositoryGeneric<Cart> cartRepository, IMapper mapper,
            IRepositoryGeneric<CartProducts> cartProductsRepository, IRabbitMqService rabbitMqService)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _cartProductsRepository = cartProductsRepository;
            _rabbitMqService = rabbitMqService;
        }

        public async Task DeleteAsync(Cart cart)
        {
            await _cartRepository.DeleteAsync(cart);
        }

        //Change!!!
        public async Task<IEnumerable<CartViewModel>> GetAllAsync()
        {
            var orders = await _cartRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<CartViewModel>>(orders);
        }

        public async Task<Cart> GetById(int id)
        {
            var order = await _cartRepository.GetByIdAsync(id);
            return order;
        }

        public async Task<IEnumerable<Cart>> GetAllByUser(Guid id)
        {
            var orders = await _cartRepository.GetByPredicate(_ => _.UserId == id);
            return orders;
        }

        public async Task AddToCart(AddToCartModel model)
        {
            var hasActiveOrder = await HasNonLockedCart(model.UserId);
            if (hasActiveOrder)
            {
                await AddProductToExistingCart(model);
            }
            else
            {
                await AddProductToNewCart(model);
            }
        }

        public async Task ChangeQuantity(int id, int quantity)
        {
            var product = await _cartProductsRepository.GetByIdAsync(id);
            product.Quantity = quantity;
            await _cartProductsRepository.UpdateAsync(product);
        }

        public async Task RemoveCartLine(int id)
        {
            var product = await _cartProductsRepository.GetByIdAsync(id);
            await _cartProductsRepository.DeleteAsync(product);
        }

        public async Task<CartViewModel> GetActiveCartByUser(Guid userId)
        {
            var order = await GetNonLockedByUser(userId);

            if (order == null) return null;
            var products = _mapper.Map<IEnumerable<ProductsRequestModel>>(order.Products.Where(_ => _.DeletedDate == null));
            var productsResponse =
                JsonConvert.DeserializeObject<IEnumerable<ProductsResponseModel>>(
                    await _rabbitMqService.SendAsync(products))?.ToList();

            var totalPrice = await CalculateTotalPrice(productsResponse);
            order.TotalPrice = totalPrice;
            await _cartRepository.UpdateAsync(order);
            var orderView = new CartViewModel()
            {
                Id = order.Id,
                UserId = order.UserId,
                Products = productsResponse,
                TotalPrice = totalPrice
            };

            return orderView;
        }

        public async Task LockCart(int orderId)
        {
            var order = await _cartRepository.GetByIdAsync(orderId);
            order.Locked = true;
            await _cartRepository.UpdateAsync(order);
        }

        #region PrivateMethods
        private async Task<Cart> GetNonLockedByUser(Guid id)
        {
            var order =
                (await _cartRepository.GetByPredicate(_ => _.UserId == id && _.DeletedDate == null && !_.Locked))
                .FirstOrDefault();
            return order;
        }

        private static async Task<decimal> CalculateTotalPrice(IEnumerable<ProductsResponseModel> products)
        {
            return await Task.Run(() => products.Sum(product => product.TotalPrice));
        }

        private async Task<bool> HasNonLockedCart(Guid id)
        {
            return (await _cartRepository.GetByPredicate(_ => _.UserId == id && _.DeletedDate == null && !_.Locked))
                .Any();
        }

        private async Task AddProductToNewCart(AddToCartModel model)
        {
            Cart cart = new()
            {
                UserId = model.UserId
            };
            var activeOrder = (await _cartRepository.CreateAsync(cart)).Entity;

            CartProducts products = new()
            {
                OrderId = activeOrder.Id,
                ProductId = model.Id,
                Quantity = model.Quantity
            };
            await _cartProductsRepository.CreateAsync(products);
        }

        private async Task AddProductToExistingCart(AddToCartModel model)
        {
            var order = await GetNonLockedByUser(model.UserId);
            var currentProduct =
                (await _cartProductsRepository.GetByPredicate(
                    _ => _.OrderId == order.Id && _.ProductId == model.Id)).FirstOrDefault();
            if (currentProduct != null)
            {
                if (currentProduct.DeletedDate != null)
                {
                    currentProduct.DeletedDate = null;
                    currentProduct.Quantity = 1;
                    await _cartProductsRepository.UpdateAsync(currentProduct);
                }
                else
                {
                    currentProduct.Quantity += model.Quantity;
                    await _cartProductsRepository.UpdateAsync(currentProduct);
                }

            }
            else
            {
                CartProducts products = new()
                {
                    OrderId = order.Id,
                    ProductId = model.Id,
                    Quantity = model.Quantity
                };

                await _cartProductsRepository.CreateAsync(products);
            }
        }
        #endregion
    }
}