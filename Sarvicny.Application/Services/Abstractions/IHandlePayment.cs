using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IHandlePayment
    {
        public Task<Response<object>> validateOrder(string transactionPaymentId, bool transactionStatus, string transactionID, string saleID,
            PaymentMethod paymentMethod);
    }
}
