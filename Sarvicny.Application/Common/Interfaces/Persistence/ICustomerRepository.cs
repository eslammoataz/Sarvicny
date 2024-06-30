using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using System.Threading.Tasks;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface ICustomerRepository
    {
        
        

        Task<Customer?> GetCustomerById(ISpecifications<Customer> specifications);

        Task<CartServiceRequest> GetCartServiceRequestById(ISpecifications<CartServiceRequest> spec);

        Task<List<CartServiceRequest>> GetReAssignedCartServiceRequest(ISpecifications<CartServiceRequest> spec, string customerId);
        Task AddRequest(CartServiceRequest newRequest);
        Task RemoveRequest(CartServiceRequest specificRequest);
        Task EmptyCart(Cart cart);

        bool CreateCart(string customerID);

        Task AddFavProvider(FavProvider favourite);
        Task RemoveFavProvider(FavProvider fav);

        
    }
}
