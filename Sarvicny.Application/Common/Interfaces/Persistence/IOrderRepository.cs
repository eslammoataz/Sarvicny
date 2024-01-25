using Microsoft.EntityFrameworkCore;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderById(string OrderId, ISpecifications<Order> specifications);


        Task<object> ShowOrderDetails(string orderId);
        Task<object> ApproveOrder(string orderId, ISpecifications<Order> spec);
        Task<object> RejectOrder(string orderId ,ISpecifications<Order> spec);
        Task<object> CancelOrder(string orderId, ISpecifications<Order> spec);
    }

}
