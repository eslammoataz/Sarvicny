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
        Task  ApproveOrder(Order order);
        Task RejectOrder(Order order);
        Task CancelOrder(Order order);
        //Task<object> ShowOrderDetails(ISpecifications<Order> spec);
        Task<Order> AddOrder(Order order);
       Task<List<Order>> GetAllOrders(ISpecifications<Order> spec);
       
       Task<List<ServiceRequest>> SetOrderToServiceRequest(List<ServiceRequest>serviceRequests , Order order);
    }

}
