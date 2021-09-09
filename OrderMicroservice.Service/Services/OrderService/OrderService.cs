using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderMicroservice.Domain.Entities;
using OrderMicroservice.Domain.Enums;
using OrderMicroservice.Repository.Repository;
using OrderMicroservice.Service.Services.Models;
using OrderMicroservice.Service.Services.RabbitMqService;

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

        public async Task CreateAsync(CreateOrderModel model)
        {
            var order = new Order
            {
                UserId = model.UserId,
                AddressId = model.AddressId,
                State = OrderStates.New,
                CartId = model.CartId,
                TotalPrice = model.TotalPrice
            };

            await _orderRepository.CreateAsync(order);
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

            var products = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(await _rabbitMqService.GetCartInfo(order.CartId));

            var address =
                JsonConvert.DeserializeObject<AddressViewModel>(await _rabbitMqService.GetAddressInfo(order.AddressId));

            var orderView = new OrderViewModel
            {
                UserId = order.UserId,
                Products = products,
                Address = address,
            };

            return orderView;
        }

        public async Task<IEnumerable<Order>> GetByUserId(Guid userId)
        {
            var orders = await _orderRepository.GetByPredicate(_ => _.UserId == userId);
            return orders;
        }
    }
}
