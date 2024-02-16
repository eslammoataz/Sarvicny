using Sarvicny.Contracts;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IHandlePayment
    {
        public Task<Response<object>> validateOrder(string orderId, bool transactionStatus);
    }
}
