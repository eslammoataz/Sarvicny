using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface ITransactionPaymentRepository
    {
        Task<TransactionPayment> AddTransactionPaymentAsync(TransactionPayment transactionPayment);
        Task<TransactionPayment> GetTransactionPaymentAsync(string transactionPaymentId);
        Task UpdateTransactionPaymentAsync(TransactionPayment transactionPayment);
        Task DeleteTransactionPaymentAsync(TransactionPayment transactionPayment);
    }
}
