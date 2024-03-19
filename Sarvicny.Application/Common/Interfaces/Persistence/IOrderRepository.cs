using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrder(ISpecifications<Order> specifications);


        Task ApproveOrder(Order order);
        Task RejectOrder(Order order);
        Task CancelOrder(Order order);
        //Task<object> ShowOrderDetails(ISpecifications<Order> spec);
        Task<Order> AddOrder(Order order);
        Task<List<Order>> GetAllOrders(ISpecifications<Order> spec);

        Task ChangeOrderStatus(Order order, string transactionId, PaymentMethod paymentMethod, bool transactionStatus);
        Task<List<ServiceRequest>> SetOrderToServiceRequest(List<ServiceRequest> serviceRequests, Order order);

        Task<OrderRating> AddCustomerRating(OrderRating rating);
        Task<OrderRating> AddServiceProviderRating(OrderRating rating);
    }

}
