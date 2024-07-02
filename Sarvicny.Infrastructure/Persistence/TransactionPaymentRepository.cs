using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
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

        public async Task<Customer> GetCustomerByTransactionPaymentId(string transactionPaymentId)
        {
            var transactionPayment = _context.TransactionPayment
                .Include(t => t.OrderList)
                .FirstOrDefault(t => t.TransactionPaymentID == transactionPaymentId);

            var customer = await _context.Customers.FindAsync(transactionPayment.OrderList.FirstOrDefault().CustomerID);

            return customer;
        }

        public async Task<TransactionPayment> GetTransactionPaymentAsync(string transactionPaymentId)
        {
            var transactionPayment = _context.TransactionPayment
                     .Include(t => t.OrderList)
                         .ThenInclude(o => o.OrderDetails)
                     .Include(t => t.OrderList)
                         .ThenInclude(o => o.Customer)
                    .Include(t => t.OrderList)
                    .ThenInclude(o => o.OrderDetails)
                            .ThenInclude(od => od.RequestedSlot)
                     .FirstOrDefault(t => t.TransactionPaymentID == transactionPaymentId);

            return transactionPayment;
        }

        public async Task UpdateTransactionPaymentAsync(TransactionPayment transactionPayment)
        {
            throw new NotImplementedException();
        }
    }
}
