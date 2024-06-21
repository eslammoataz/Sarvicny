using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrder(ISpecifications<Order> specifications);


        Task ApproveOrder(Order orderId);
        Task RejectOrder(Order orderId);
        Task CancelOrder(Order orderId);

        
        Task<Order> AddOrder(Order order);
        Task<OrderDetails> AddOrderDetails(OrderDetails orderDetails);
        Task<RequestedSlot> AddRequestedSlot(RequestedSlot requestedSlot);

        Task<List<Order>> GetAllOrders(ISpecifications<Order> spec);

        Task<List<Order>> GetAllOrdersForProvider(ISpecifications<Order> spec, string providerId);
        Task<List<Order>> GetAllPendingOrdersForProvider(ISpecifications<Order> spec, string providerId);
        Task<List<Order>> GetAllApprovedOrdersForProvider(ISpecifications<Order> spec, string providerId);

        Task<List<Order>> GetAllRejectedOrders(ISpecifications<Order> spec);
        Task<List<Order>> GetAllPendingOrders(ISpecifications<Order> spec);
        Task<List<Order>> GetAllCanceledOrders(ISpecifications<Order> spec);
        Task<List<Order>> GetAllApprovedOrders(ISpecifications<Order> spec);

        Task ChangeOrderPaidStatus(Order order, string transactionId, PaymentMethod paymentMethod, bool transactionStatus);

        Task<OrderRating> AddRating(OrderRating rate);


        //Task<List<ProviderRating>> GetAllProviderRating();
        //Task<List<OrderRating>> GetAllCustomerRating();





    }

}
