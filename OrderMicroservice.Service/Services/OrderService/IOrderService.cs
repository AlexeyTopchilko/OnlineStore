using OrderMicroservice.Service.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderMicroservice.Service.Services.OrderService
{
    public interface IOrderService
    {
        Task DeleteByIdAsync(int id);

        Task<OrderViewModel> GetByIdAsync(int id);

        Task<IEnumerable<UserOrdersViewModel>> GetByUserId(Guid userId);

        Task<int> FormAnOrder(FormAnOrderModel model);

        Task ConfirmOrder(int orderId);

        Task TakePayment(PaymentResult result);
    }
}