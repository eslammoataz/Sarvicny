using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Email;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Infrastructure.Persistence
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;


        public OrderRepository(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        //public async Task ApproveOrder(Order order)
        //{

        //    order.OrderStatus = OrderStatusEnum.Approved;

        //}

        //public async Task RejectOrder(Order order)
        //{

        //    order.OrderStatus = OrderStatusEnum.Rejected;

        //}

        public async Task CancelOrder(Order order)
        {

            order.OrderStatus = OrderStatusEnum.CanceledByProvider;
        }

        private IQueryable<Order> ApplySpecification(ISpecifications<Order> spec)
        {
            return SpecificationBuilder<Order>.Build(_context.Orders, spec);
        }

        public async Task<Order?> GetOrder(ISpecifications<Order> specifications)
        {
            return await ApplySpecification(specifications).FirstOrDefaultAsync();
        }



        public async Task<Order> AddOrder(Order order)
        {

            _context.Orders.Add(order);
            return order;
        }

        public async Task<List<Order>> GetAllOrders(ISpecifications<Order> spec)
        {

            var orders = await ApplySpecification(spec).OrderByDescending(or => or.OrderDetails.RequestedSlot.RequestedDay.Date).ToListAsync();
            return orders;

        }
        public async Task<List<Order>> GetAllOrdersForProvider(ISpecifications<Order> spec, string providerId)
        {

            var orders = await ApplySpecification(spec).Where(or => or.OrderDetails.ProviderID == providerId)
                .OrderByDescending(or => or.OrderDetails.RequestedSlot.RequestedDay).ToListAsync();
            return orders;

        }

        //public async Task<List<Order>> GetAllApprovedOrdersForProvider(ISpecifications<Order> spec, string providerId)
        //{

        //    var approvedStatus = OrderStatusEnum.Approved.ToString();
        //    var orders = await ApplySpecification(spec)
        //        .Where(or => or.OrderStatusString == approvedStatus && or.OrderDetails.ProviderID == providerId)
        //        .ToListAsync();
        //    return orders;

        //}
        public async Task<List<Order>> GetAllPendingOrdersForProvider(ISpecifications<Order> spec, string providerId)
        {

            var pendingStatus = OrderStatusEnum.Pending.ToString();
            var paidStatus = OrderStatusEnum.Paid.ToString();
            var orders = await ApplySpecification(spec)
                .Where(or => or.OrderDetails.ProviderID == providerId)
                .OrderByDescending(or => or.OrderDetails.RequestedSlot.RequestedDay.Date).ToListAsync();

            var result = orders.Where(or => or.OrderStatusString == pendingStatus || or.OrderStatusString == paidStatus);
            return orders;

        }

        public async Task<List<Order>> GetAllCanceledByProviderOrders(ISpecifications<Order> spec)
        {
            var canceledStatus = OrderStatusEnum.CanceledByProvider.ToString();
            var orders = await ApplySpecification(spec)
                .Where(or => or.OrderStatusString == canceledStatus)
                .OrderByDescending(or => or.OrderDetails.RequestedSlot.RequestedDay.Date).ToListAsync();
            return orders;


        }
        public async Task<List<Order>> GetAllPendingOrPaidOrders(ISpecifications<Order> spec)
        {

            var PendingStatus = OrderStatusEnum.Pending.ToString();
            var paid = OrderStatusEnum.Paid.ToString();

            var orders = await ApplySpecification(spec)
                .Where(or => or.OrderStatusString == PendingStatus || or.OrderStatusString == paid)
                .OrderByDescending(or => or.OrderDetails.RequestedSlot.RequestedDay.Date)
                .ToListAsync();
            return orders;

        }
        //public async Task<List<Order>> GetAllApprovedOrders(ISpecifications<Order> spec)
        //{
        //    var approvedStatus = OrderStatusEnum.Approved.ToString();
        //    var orders = await ApplySpecification(spec)
        //        .Where(or => or.OrderStatusString == approvedStatus)
        //        .ToListAsync();
        //    return orders;
        //}
        //public async Task<List<Order>> GetAllRejectedOrders(ISpecifications<Order> spec)
        //{

        //    var RejectedStatus = OrderStatusEnum.Rejected.ToString();
        //    var orders = await ApplySpecification(spec)
        //        .Where(or => or.OrderStatusString == RejectedStatus)
        //        .ToListAsync();
        //    return orders;

        //}


        public async Task ChangeOrderPaidStatus(Order order, string transactionId, string saleId, PaymentMethod PaymentMethod, bool TransactionStatus)
        {
            order.IsPaid = TransactionStatus;
            //order.TransactionID = transactionId;
            //order.PaymentMethod = PaymentMethod;
            //order.SaleID = saleId;
            return;
        }




        public async Task<OrderRating> AddRating(OrderRating rate)
        {
            await _context.OrderRatings.AddAsync(rate);
            return rate;
        }

        public async Task<OrderDetails> AddOrderDetails(OrderDetails orderDetails)
        {

            await _context.OrderDetails.AddAsync(orderDetails);
            return orderDetails;

        }

        public async Task<RequestedSlot> AddRequestedSlot(RequestedSlot requestedSlot)
        {
            await _context.RequestedSlots.AddAsync(requestedSlot);
            return requestedSlot;

        }

        //public async Task<List<Order>> GetAllExpiredOrders(ISpecifications<Order> spec)
        //{
        //    var orders = await ApplySpecification(spec)
        //           .Where(or => or.ExpiryDate != null && or.ExpiryDate < DateTime.UtcNow)
        //           .ToListAsync();
        //    return orders;
        //}




        //public async Task<List<Order>> getAllPaymentExpiredOrders(ISpecifications<Order> spec) //case ano al paymeny expiry date 3da we m7awlsh yedf3 fa al order mt3mlosh remove fa al admin yeshelo
        //{

        //    var removed = OrderStatusEnum.Removed.ToString();
        //    var orders = await ApplySpecification(spec)
        //            .Where(or => or.OrderStatusString != removed && or.TransactionPayment.PaymentExpiryTime != null && or.TransactionPayment.PaymentExpiryTime < DateTime.UtcNow)
        //            .OrderByDescending(or => or.OrderDetails.RequestedSlot.RequestedDay.Date)
        //            .ToListAsync();

        //    return orders;


        //}


    }
}
