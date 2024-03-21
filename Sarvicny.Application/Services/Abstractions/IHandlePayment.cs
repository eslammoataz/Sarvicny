using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IHandlePayment
    {
        public Task<Response<object>> validateOrder(string orderId, bool transactionStatus, string transactionID,
            PaymentMethod paymentMethod);
    }
}
