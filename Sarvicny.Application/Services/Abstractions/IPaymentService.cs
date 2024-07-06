using Sarvicny.Contracts;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IPaymentService
    {
        public Task<Response<object>> PayOrder(string TransactionId);
        Task<Response<object>> RefundAffectedTransaction(string transactionPaymentId);
    }
}
