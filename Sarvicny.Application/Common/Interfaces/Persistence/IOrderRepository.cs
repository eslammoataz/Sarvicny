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
        Task<Order?> GetOrder(ISpecifications<Order> specifications);
        Task<Order> ApproveOrder(ISpecifications<Order> spec);
        Task<object> RejectOrder(ISpecifications<Order> spec);
        Task<object> CancelOrder(ISpecifications<Order> spec);
        Task<object> ShowOrderDetails(ISpecifications<Order> spec);
        Task<Order> AddOrder(Order order);
       Task<List<Order>> GetAllOrders(ISpecifications<Order> spec);
    }

}
