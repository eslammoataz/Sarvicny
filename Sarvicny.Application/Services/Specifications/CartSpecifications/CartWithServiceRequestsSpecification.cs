using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.CartSpecifications;

public class CartWithServiceRequestsSpecification : BaseSpecifications<Cart>
{
    public CartWithServiceRequestsSpecification()
    {
        Includes.Add(c => c.ServiceRequests);
    }

    public CartWithServiceRequestsSpecification(string cartId) : base(c => c.CartID == cartId)
    {
        Includes.Add(c => c.ServiceRequests);
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}");
    }
    
}