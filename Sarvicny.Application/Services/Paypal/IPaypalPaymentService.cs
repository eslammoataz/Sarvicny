using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Paypal
{
    public interface IPaypalPaymentService
    {
        public Task<string> GetAuthToken();
        public Task<object> CreateOrder(Order order);
        public Task<object> CaptureOrder(string orderId);

        public Task<Response<object>> Pay(TransactionPayment order);
        public Task<Response<object>> ExecutePayment(string paymentId, string payerId, string token);

        public Task<object> GetPayment(string paymentId);

        public Task<Response<object>> Refund(TransactionPayment transactionPayment, List<Order> orders, decimal amount);
    }
}
