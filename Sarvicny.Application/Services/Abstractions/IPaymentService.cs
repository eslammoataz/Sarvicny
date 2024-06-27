using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IPaymentService
    {
        public Task<Response<object>> PayOrder(Order order, PaymentMethod paymentMethod);
        Task<Response<object>> RefundOrder(Order order, decimal amount);
    }
}
