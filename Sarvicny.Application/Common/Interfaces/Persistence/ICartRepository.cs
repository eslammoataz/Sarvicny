using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence;

public interface ICartRepository
{
    Task<Cart> GetCart(ISpecifications<Cart> specifications);
    
}