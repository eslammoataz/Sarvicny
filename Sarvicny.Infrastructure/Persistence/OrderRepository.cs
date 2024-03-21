using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Email;
using Sarvicny.Domain.Entities;
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


        //public async Task<object> ShowOrderDetails(string orderId, ISpecifications<Order> spec)
        //{
        //    var order = await ApplySpecification(spec).FirstOrDefaultAsync(o => o.OrderID == orderId);

        //    var customer = order.Customer;

        //    var response = new
        //    {
        //        CustomerData = new
        //        {
        //            customer.FirstName,
        //            customer.Address,
        //            // Add other customer properties as needed
        //        },
        //        ServiceData = customer.Cart.ServiceRequests.Select(s => new
        //        {
        //            ServiceId = s.providerService.Service.ServiceName
        //        }).ToList<object>(),
        //    };
        //    return response;
        //}

        public async Task ApproveOrder(Order order)
        {

            order.OrderStatus = OrderStatusEnum.Approved;

        }

        public async Task RejectOrder(Order order)
        {

            order.OrderStatus = OrderStatusEnum.Rejected;

        }

        public async Task CancelOrder(Order order)
        {

            order.OrderStatus = OrderStatusEnum.Canceled;
        }

        private IQueryable<Order> ApplySpecification(ISpecifications<Order> spec)
        {
            return SpecificationBuilder<Order>.Build(_context.Orders, spec);
        }

        private IQueryable<ServiceRequest> ApplySpecificationSR(ISpecifications<ServiceRequest> spec)
        {
            return SpecificationBuilder<ServiceRequest>.Build(_context.ServiceRequests, spec);
        }
        public async Task<Order?> GetOrder(ISpecifications<Order> specifications)
        {
            return await ApplySpecification(specifications).FirstOrDefaultAsync();
        }


        public async Task<object> RejectOrder(ISpecifications<Order> spec)
        {
            throw new NotImplementedException();
        }

        public async Task<object> CancelOrder(ISpecifications<Order> spec)
        {
            throw new NotImplementedException();
        }

        //public async Task<object> ShowOrderDetails(ISpecifications<Order> spec)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<Order> AddOrder(Order order)
        {

            var orderStatus = _context.OrderStatuses;
            order.OrderStatus = OrderStatusEnum.Pending;


            _context.Orders.Add(order);
            return order;
        }

        public async Task<List<Order>> GetAllOrders(ISpecifications<Order> spec)
        {

            var orders = await ApplySpecification(spec).ToListAsync();
            return orders;

        }

        public async Task<List<ServiceRequest>> SetOrderToServiceRequest(List<ServiceRequest> serviceRequests, Order order)
        {
            foreach (var serviceRequest in serviceRequests)
            {
                serviceRequest.OrderId = order.OrderID;
            }
            return serviceRequests;
        }

        public async Task ChangeOrderStatus(Order order, string transactionId, PaymentMethod PaymentMethod, bool TransactionStatus)
        {
            order.IsPaid = TransactionStatus;
            order.TransactionID = transactionId;
            order.PaymentMethod = PaymentMethod;
            return;
        }





       
        public async Task<ServiceRequest> GetServiceRequestByID(ISpecifications<ServiceRequest> spec)
        {
            return await ApplySpecificationSR(spec).FirstOrDefaultAsync();
        }

        public async Task<CustomerRating> AddCustomerRating(CustomerRating rate)
        {
           _context.customerRatings.AddAsync(rate);
            return rate;
        }
         

        public async Task<ProviderRating> AddProviderRating(ProviderRating rate)
        {
          _context.ProviderRatings.AddAsync(rate);
            return rate;
        }

        public async Task<List<ProviderRating>> GetAllProviderRating()
        {
          return await _context.ProviderRatings.ToListAsync();
        }

        public async Task<List<CustomerRating>> GetAllCustomerRating()
        {
            return await _context.customerRatings.ToListAsync();
        }
    }
}
