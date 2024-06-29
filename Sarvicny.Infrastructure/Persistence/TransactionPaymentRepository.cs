using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Infrastructure.Persistence
{
    public class TransactionPaymentRepository : ITransactionPaymentRepository
    {
        private readonly AppDbContext _context;

        public TransactionPaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionPayment> AddTransactionPaymentAsync(TransactionPayment transactionPayment)
        {
            await _context.TransactionPayment.AddAsync(transactionPayment);
            return transactionPayment;
        }

        public async Task DeleteTransactionPaymentAsync(TransactionPayment transactionPayment)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionPayment> GetTransactionPaymentAsync(string transactionPaymentId)
        {
            var transactionPayment = await _context.TransactionPayment.FindAsync(transactionPaymentId);
            return transactionPayment;
        }

        public async Task UpdateTransactionPaymentAsync(TransactionPayment transactionPayment)
        {
            throw new NotImplementedException();
        }
    }
}
