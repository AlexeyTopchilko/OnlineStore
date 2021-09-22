using Newtonsoft.Json;
using OrderMicroservice.Domain.Entities;
using OrderMicroservice.Domain.Enums;
using OrderMicroservice.Repository.Repository;
using OrderMicroservice.Service.Services.Models;
using OrderMicroservice.Service.Services.RabbitMqService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderMicroservice.Service.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryGeneric<Order> _orderRepository;
        private readonly IRabbitMqService _rabbitMqService;

        public OrderService(IRepositoryGeneric<Order> orderRepository, IRabbitMqService rabbitMqService)
        {
            _orderRepository = orderRepository;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<int> FormAnOrder(FormAnOrderModel model)
        {
            var order = (await _orderRepository.GetByPredicate(_ => _.CartId == model.CartId)).FirstOrDefault();
            if (order == null)
            {
                var orderId = await CreateOrder(model);
                return orderId;
            }
            else
            {
                var totalPrice = await _rabbitMqService.GetTotalPrice(model.CartId);
                order.AddressId = model.AddressId;
                order.TotalPrice = decimal.Parse(totalPrice);
                await _orderRepository.UpdateAsync(order);
                return order.Id;
            }
        }

        public async Task DeleteByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            await _orderRepository.DeleteAsync(order);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders;
        }

        public async Task<OrderViewModel> GetByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order != null)
            {
                var productsInfo = await _rabbitMqService.GetProductsInfo(order.CartId);
                var products = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(productsInfo);

                var address =
                    JsonConvert.DeserializeObject<AddressViewModel>(await _rabbitMqService.GetAddressInfo(order.AddressId));

                var orderView = new OrderViewModel
                {
                    Id = id,
                    UserId = order.UserId,
                    Products = products,
                    Address = address,
                    TotalPrice = order.TotalPrice
                };

                return orderView;
            }
            else
                return null;
        }

        public async Task<IEnumerable<UserOrdersViewModel>> GetByUserId(Guid userId)
        {
            var orders = await _orderRepository.GetByPredicate(_ => _.UserId == userId);
            var ordersView = new List<UserOrdersViewModel>();
            foreach (var order in orders)
            {
                if (order == null) continue;

                //var products = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(await _rabbitMqService.GetProductsInfo(order.CartId));

                var address =
                    JsonConvert.DeserializeObject<AddressViewModel>(await _rabbitMqService.GetAddressInfo(order.AddressId));
                var orderView = new UserOrdersViewModel
                {
                    Id = order.Id,
                    Address = address.City+","+address.Street+","+address.HouseNumber,
                    TotalPrice = order.TotalPrice,
                    State = order.State.ToString()
                };

                ordersView.Add(orderView);
            }

            return ordersView;
        }

        public async Task ConfirmOrder(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            order.State = OrderStates.Processing;
            await _orderRepository.UpdateAsync(order);

            _rabbitMqService.LockTheCart(order.CartId);
        }

        private async Task<int> CreateOrder(FormAnOrderModel model)
        {
            var totalPrice = await _rabbitMqService.GetTotalPrice(model.CartId);
            var order = new Order
            {
                UserId = model.UserId,
                AddressId = model.AddressId,
                State = OrderStates.New,
                CartId = model.CartId,
                TotalPrice = decimal.Parse(totalPrice)
            };

            var orderId = (await _orderRepository.CreateAsync(order)).Entity.Id;
            return orderId;
        }
    }
}