using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Infrastructure.Persistence;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;
    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task ClearCart(Cart cart)
    {
        var requests = cart.CartServiceRequests;
        foreach (var request in requests)
        {
            _context.CartServiceRequests.Remove(request);
        }
        
    }

    public async Task<Cart> GetCart(ISpecifications<Cart> specifications)
    {
        return await ApplySpecification(specifications).FirstOrDefaultAsync();
    }


    private IQueryable<Cart> ApplySpecification(ISpecifications<Cart> spec)
    {
        return SpecificationBuilder<Cart>.Build(_context.Carts, spec);
    }

}