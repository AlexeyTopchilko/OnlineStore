﻿using OrderMicroservice.Service.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderMicroservice.Service.Services.OrderService
{
    public interface IOrderService
    {
        Task DeleteByIdAsync(int id);

        Task<IEnumerable<Domain.Entities.Order>> GetAllAsync();

        Task<OrderViewModel> GetByIdAsync(int id);

        Task<IEnumerable<Domain.Entities.Order>> GetByUserId(Guid userId);

        Task CreateAsync(CreateOrderModel model);
    }
}