using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Email;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Infrastructure.Persistence
{
    public class OrderRepositry : IOrderRepository
    {
        private readonly AppDbContext _context;

        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailService _emailService;


        public OrderRepositry(AppDbContext context, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _context = context;
            this.unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<Order> GetOrderById(string OrderId, ISpecifications<Order> specifications)
        {

            return await ApplySpecification(specifications).FirstOrDefaultAsync(o => o.OrderID == OrderId);
        }


        public async Task<object> ShowOrderDetails(string orderId, ISpecifications<Order> spec)
        {
            var order = await ApplySpecification(spec).FirstOrDefaultAsync(o => o.OrderID == orderId);

            var customer = order.Customer;

            var response = new
            {
                CustomerData = new
                {
                    customer.FirstName,
                    customer.Address,
                    // Add other customer properties as needed
                },
                ServiceData = customer.Cart.ServiceRequests.Select(s => new
                {
                    ServiceId = s.providerService.Service.ServiceName
                }).ToList<object>(),
            };
            return response;
        }

        public async Task<object> ApproveOrder(string orderId, ISpecifications<Order> spec)
        {
            var order = await ApplySpecification(spec).FirstOrDefaultAsync(o => o.OrderID == orderId);

            //if (order == null)
            //{
            //    throw new Exception("Order Not Found");

            //}

            var customer = order.Customer;

            //al a7sn yeb2a enum bs ana 7alian bzwdha fe al db
            order.OrderStatusID = "2";
            order.OrderStatus = _context.OrderStatuses.FirstOrDefault(o => o.StatusName == "Approved");


            unitOfWork.Commit();

            var message = new EmailDto(customer.Email!, "Sarvicny: Approved", "Your order is approved seccessfully");

            _emailService.SendEmail(message);
            unitOfWork.Commit();
            return order;
        }

        public async Task<object> RejectOrder(string orderId, ISpecifications<Order> spec)
        {
            var order = await ApplySpecification(spec).FirstOrDefaultAsync(o => o.OrderID == orderId);

            var customer = order.Customer;

            //al a7sn yeb2a enum bs ana 7alian bzwdha fe al db
            order.OrderStatusID = "3";
            order.OrderStatus = _context.OrderStatuses.FirstOrDefault(o => o.StatusName == "Rejected");
            unitOfWork.Commit();

            var message = new EmailDto(customer.Email!, "Sarvicny: Rejected", "Your order is Rejected ");

            _emailService.SendEmail(message);
            unitOfWork.Commit();
            ///ReAsignnnnnn??
            return order;

        }

        public async Task<object> CancelOrder(string orderId, ISpecifications<Order> spec)
        {
            var order = await ApplySpecification(spec).FirstOrDefaultAsync(o => o.OrderID == orderId);

            //// M7Taga t check al condition da fe al serviceeee
            ///


            if (order.OrderStatusID == "2") //Approved
            {

                //al a7sn yeb2a enum bs ana 7alian bzwdha fe al db
                order.OrderStatusID = "4";
                order.OrderStatus = _context.OrderStatuses.FirstOrDefault(o => o.StatusName == "Canceled");
            }


            else
            {
                return null;
                // throw new Exception("Order was not originally approved to be Canceled");
            }
            var customer = order.Customer;

            var message = new EmailDto(customer.Email!, "Sarvicny: Canceled", "Unfortunally your order is canceled");

            _emailService.SendEmail(message);

            unitOfWork.Commit();

            ///ReAsignnnnnn??
            return order;

        }

        private IQueryable<Order> ApplySpecification(ISpecifications<Order> spec)
        {
            return SpecificationBuilder<Order>.Build(_context.Orders, spec);
        }


        public async Task<Order> GetOrderById(ISpecifications<Order> specifications)
        {
            throw new NotImplementedException();
        }

        public async Task<object> ApproveOrder(ISpecifications<Order> spec)
        {
            throw new NotImplementedException();
        }

        public async Task<object> RejectOrder(ISpecifications<Order> spec)
        {
            throw new NotImplementedException();
        }

        public async Task<object> CancelOrder(ISpecifications<Order> spec)
        {
            throw new NotImplementedException();
        }

        public async Task<object> ShowOrderDetails(ISpecifications<Order> spec)
        {
            throw new NotImplementedException();
        }
    }
}
