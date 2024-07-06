using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface ITransactionPaymentRepository
    {
        Task<TransactionPayment> AddTransactionPaymentAsync(TransactionPayment transactionPayment);
        Task<TransactionPayment> GetTransactionPaymentAsync(string transactionPaymentId); // to get transaction payment by transaction payment id

        Task<TransactionPayment?> GetTransactionPaymentByIdAsync(ISpecifications<TransactionPayment> spec); // to get transaction payment by transaction payment id
        Task<List<TransactionPayment>> getAllRefundableTransactions(ISpecifications<TransactionPayment> spec);
        Task UpdateTransactionPaymentAsync(TransactionPayment transactionPayment);
        Task DeleteTransactionPaymentAsync(TransactionPayment transactionPayment);

        Task<Customer> GetCustomerByTransactionPaymentId(string transactionPaymentId);

        Task<TransactionPayment?> GetTransactionByTransactionID(string transactionId); // for getting transaction payment from transaction id
    }
}
