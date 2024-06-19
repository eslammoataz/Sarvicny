using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface ICustomerRepository
    {
        Task<Cart> GetCart(ISpecifications<Cart> specifications);

        Task<Customer?> GetCustomerById(ISpecifications<Customer> specifications);

        Task<CartServiceRequest> GetCartServiceRequestById(ISpecifications<CartServiceRequest> spec);
        Task AddRequest(CartServiceRequest newRequest);
        Task RemoveRequest(CartServiceRequest specificRequest);
        Task EmptyCart(Cart cart);

        bool CreateCart(string customerID);
        List<object> GetServiceRequests();
    }
}
