using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrder(ISpecifications<Order> specifications);


        //Task ApproveOrder(Order orderId);
        //Task RejectOrder(Order orderId);
        Task CancelOrder(Order orderId);


        Task<Order> AddOrder(Order order);
        Task<OrderDetails> AddOrderDetails(OrderDetails orderDetails);
        Task<RequestedSlot> AddRequestedSlot(RequestedSlot requestedSlot);

        Task<List<Order>> GetAllOrders(ISpecifications<Order> spec);

        Task<List<Order>> GetAllOrdersForProvider(ISpecifications<Order> spec, string providerId);
        Task<List<Order>> GetAllPendingOrdersForProvider(ISpecifications<Order> spec, string providerId);
        //Task<List<Order>> GetAllApprovedOrdersForProvider(ISpecifications<Order> spec, string providerId);

        //Task<List<Order>> GetAllRejectedOrders(ISpecifications<Order> spec);
        Task<List<Order>> GetAllPendingOrPaidOrders(ISpecifications<Order> spec);
        Task<List<Order>> GetAllCanceledByProviderOrders(ISpecifications<Order> spec);
        //Task<List<Order>> GetAllApprovedOrders(ISpecifications<Order> spec);

        //Task<List<Order>> GetAllExpiredOrders(ISpecifications<Order> spec);

        //Task<List<Order>> getAllPaymentExpiredOrders(ISpecifications<Order> spec);
        // Task<List<Order>> getAllExpiredOrders(ISpecifications<Order> spec);

        Task<List<Order>> GetAllCanceled_Reassigned_RemovedWithRefundOrders(ISpecifications<Order> spec);

        Task ChangeOrderPaidStatus(Order order, string transactionId, string saleId, PaymentMethod paymentMethod, bool transactionStatus);

        Task<OrderRating> AddRating(OrderRating rate);

    }

}
