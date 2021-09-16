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

        public async Task<IEnumerable<CartViewModel>> GetAllAsync()
        {
            var carts = await _cartRepository.GetAllAsync();
            var cartsView = new List<CartViewModel>();
            foreach (var cart in carts)
            {
                if (cart == null) continue;
                var products =
                    _mapper.Map<IEnumerable<ProductsRequestModel>>(cart.Products.Where(_ => _.DeletedDate == null));
                var productsResponse =
                    JsonConvert.DeserializeObject<IEnumerable<ProductsResponseModel>>(
                        await _rabbitMqService.SendAsync(products))?.ToList();

                var totalPrice = await CalculateTotalPrice(productsResponse);
                cart.TotalPrice = totalPrice;
                await _cartRepository.UpdateAsync(cart);
                var cartView = new CartViewModel()
                {
                    Id = cart.Id,
                    UserId = cart.UserId,
                    Products = productsResponse,
                    TotalPrice = totalPrice
                };
                cartsView.Add(cartView);
            }

            return cartsView;
        }

        public async Task<Cart> GetById(int id)
        {
            var cart = await _cartRepository.GetByIdAsync(id);
            return cart;
        }

        public async Task<IEnumerable<Cart>> GetAllByUser(Guid id)
        {
            var carts = await _cartRepository.GetByPredicate(_ => _.UserId == id);
            return carts;
        }

        public async Task AddToCart(AddToCartModel model)
        {
            var hasActiveCart = await HasNonLockedCart(model.UserId);
            if (hasActiveCart)
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
            var cart = await GetNonLockedByUser(userId);

            if (cart == null) return null;
            var products = _mapper.Map<IEnumerable<ProductsRequestModel>>(cart.Products.Where(_ => _.DeletedDate == null));
            var productsResponse =
                JsonConvert.DeserializeObject<IEnumerable<ProductsResponseModel>>(
                    await _rabbitMqService.SendAsync(products))?.ToList();

            var totalPrice = await CalculateTotalPrice(productsResponse);
            cart.TotalPrice = totalPrice;
            await _cartRepository.UpdateAsync(cart);
            var orderView = new CartViewModel()
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Products = productsResponse,
                TotalPrice = totalPrice
            };

            return orderView;
        }

        public async Task LockTheCart(int cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            cart.Locked = true;
            await _cartRepository.UpdateAsync(cart);
        }

        #region PrivateMethods
        private async Task<Cart> GetNonLockedByUser(Guid id)
        {
            var cart =
                (await _cartRepository.GetByPredicate(_ => _.UserId == id && _.DeletedDate == null && !_.Locked))
                .FirstOrDefault();
            return cart;
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
            var activeCart = (await _cartRepository.CreateAsync(cart)).Entity;

            CartProducts products = new()
            {
                OrderId = activeCart.Id,
                ProductId = model.Id,
                Quantity = model.Quantity
            };
            await _cartProductsRepository.CreateAsync(products);
        }

        private async Task AddProductToExistingCart(AddToCartModel model)
        {
            var cart = await GetNonLockedByUser(model.UserId);
            var currentProduct =
                (await _cartProductsRepository.GetByPredicate(
                    _ => _.OrderId == cart.Id && _.ProductId == model.Id)).FirstOrDefault();
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
                    OrderId = cart.Id,
                    ProductId = model.Id,
                    Quantity = model.Quantity
                };

                await _cartProductsRepository.CreateAsync(products);
            }
        }
        #endregion
    }
}