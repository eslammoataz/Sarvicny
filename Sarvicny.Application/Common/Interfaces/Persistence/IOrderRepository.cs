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
        Task<List<OrderServiceRequest>> SetOrderToServiceRequest(List<OrderServiceRequest> serviceRequests, Order order);

        Task<CustomerRating> AddCustomerRating(CustomerRating rate);
        Task<ProviderRating> AddProviderRating(ProviderRating rating);

     
         Task<OrderServiceRequest> GetOrderServiceRequestByID(ISpecifications<OrderServiceRequest> spec);
        Task<List<ProviderRating>> GetAllProviderRating();
        Task<List<CustomerRating>> GetAllCustomerRating();





    }

}
