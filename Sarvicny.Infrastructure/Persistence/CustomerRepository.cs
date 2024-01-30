using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;

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

        public  async Task RemoveRequest(ServiceRequest specificRequest)
        {
            _context.ServiceRequests.Remove(specificRequest);
        }
    }
}
