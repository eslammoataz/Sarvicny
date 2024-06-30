using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
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
        

        public async Task<Customer?> GetCustomerById(ISpecifications<Customer> specifications)
        {
            var customer = await ApplySpecification(specifications).FirstOrDefaultAsync();

            return customer;
        }

        public async Task AddRequest(CartServiceRequest newRequest)
        {
            await _context.CartServiceRequests.AddAsync(newRequest);
        }


        private IQueryable<Customer> ApplySpecification(ISpecifications<Customer> spec)
        {
            return SpecificationBuilder<Customer>.Build(_context.Customers, spec);
        }
        private IQueryable<CartServiceRequest> ApplySpecificationS(ISpecifications<CartServiceRequest> spec)
        {
            return SpecificationBuilder<CartServiceRequest>.Build(_context.CartServiceRequests, spec);
        }

        public async Task RemoveRequest(CartServiceRequest specificRequest)
        {
            _context.CartServiceRequests.Remove(specificRequest);
        }

        public async Task EmptyCart(Cart cart)
        {
            cart.CartServiceRequests.Clear();
        }

        public async Task<CartServiceRequest> GetCartServiceRequestById(ISpecifications<CartServiceRequest> spec)
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

        public  async Task AddFavProvider(FavProvider favourite)
        {
        
            await _context.FavProviders.AddAsync(favourite);

        }

        public async Task RemoveFavProvider(FavProvider fav)
        {
             _context.FavProviders.Remove(fav);

        }

        public async Task<List<CartServiceRequest>> GetReAssignedCartServiceRequest(ISpecifications<CartServiceRequest> spec, string customerId)
        {

            return await ApplySpecificationS(spec).Where(r=> r.Cart.CustomerID==customerId && r.ReAssigned==true).ToListAsync();
        }
    }
}
