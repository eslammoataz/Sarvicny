using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;
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
                .Include(t => t.OrderList).ThenInclude(o => o.Customer)
                .FirstOrDefault(t => t.TransactionPaymentID == transactionPaymentId);

            var customer = await _context.Customers.FindAsync(transactionPayment.OrderList.FirstOrDefault().CustomerID);

            return customer;
        }

        public async Task<TransactionPayment> GetTransactionByTransactionID(string transactionId)
        {
            var transactionPayment = _context.TransactionPayment
                .Include(t => t.OrderList).ThenInclude(o => o.Customer)
                .FirstOrDefault(t => t.TransactionID == transactionId);

            return transactionPayment;
        }
        private IQueryable<TransactionPayment> ApplySpecification(ISpecifications<TransactionPayment> spec)
        {
            return SpecificationBuilder<TransactionPayment>.Build(_context.TransactionPayment, spec);
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

        public async Task<List<TransactionPayment>> getAllRefundableTransactions(ISpecifications<TransactionPayment> spec)
        {
            var CanceledStatus = OrderStatusEnum.Canceled.ToString();
            var Reassigned = OrderStatusEnum.ReAssigned.ToString();
            var RemovedWithRefund = OrderStatusEnum.RemovedWithRefund.ToString();
            var cash = PaymentMethod.Cash.ToString();

            var transactions = await ApplySpecification(spec)
    .Where(t => t.PaymentMethod != PaymentMethod.Cash)
    .Where(t => t.OrderList.Any(o => o.OrderStatusString == CanceledStatus ||
                                     o.OrderStatusString == Reassigned ||
                                     o.OrderStatusString == RemovedWithRefund)).ToListAsync();


            return transactions;
        }

        public async Task UpdateTransactionPaymentAsync(TransactionPayment transactionPayment)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionPayment?> GetTransactionPaymentByIdAsync(ISpecifications<TransactionPayment> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }
    }
}
