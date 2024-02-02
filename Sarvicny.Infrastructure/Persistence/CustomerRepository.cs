using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;
using System.Linq;

namespace Sarvicny.Infrastructure.Persistence
{
    public class CustomerRepository : ICustomerRepository

    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Cart> GetCart(ISpecifications<Cart> specifications)
        {
            throw new NotImplementedException();
        }

        public async Task<Customer?> GetCustomerById(ISpecifications<Customer> specifications)
        {
            var customer = await ApplySpecification(specifications).FirstOrDefaultAsync();

            return customer;
        }

        public async Task AddRequest(ServiceRequest newRequest)
        {
            await _context.ServiceRequests.AddAsync(newRequest);
        }


        private IQueryable<Customer> ApplySpecification(ISpecifications<Customer> spec)
        {
            return SpecificationBuilder<Customer>.Build(_context.Customers, spec);
        }
        private IQueryable<ServiceRequest> ApplySpecificationS(ISpecifications<ServiceRequest> spec)
        {
            return SpecificationBuilder<ServiceRequest>.Build(_context.ServiceRequests, spec);
        }

        public async Task RemoveRequest(ServiceRequest specificRequest)
        {
            _context.ServiceRequests.Remove(specificRequest);
        }

        public async Task EmptyCart(Cart cart)
        {
            cart.ServiceRequests.Clear();
        }

        public async Task<ServiceRequest> GetServiceRequestById(ISpecifications<ServiceRequest> spec)
        {
            return await ApplySpecificationS(spec).FirstOrDefaultAsync();

        }


        public bool CreateCart(string customerId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
            {
                return false;
            }

            var cart = new Cart
            {
                CustomerID = customerId,
                LastChangeTime = DateTime.UtcNow,
                Customer = customer
            };

            customer.Cart = cart;
            customer.CartID = cart.CartID;
            return true;
        }
        public List<object> GetServiceRequests() { 
            return  _context.ServiceRequests.Select(s=>new
            {
                s.Price,
                s.SlotID,
                s.ProviderServiceID
            }).ToList<object>();
        }
    }
}
