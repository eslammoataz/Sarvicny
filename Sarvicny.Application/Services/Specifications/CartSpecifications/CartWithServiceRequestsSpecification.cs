using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.CartSpecifications;

public class CartWithServiceRequestsSpecification : BaseSpecifications<Cart>
{
    public CartWithServiceRequestsSpecification()
    {
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.Provider)}");
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.ParentService)}");
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.Criteria)}");
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.ProviderServices)}");
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.Slot)}");
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.providerDistrict)}.{nameof(ProviderDistrict.District)}");
    }

    public CartWithServiceRequestsSpecification(string cartId) : base(c => c.CartID == cartId)
    {

        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.Provider)}");
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.ParentService)}");
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.Criteria)}");
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.ProviderServices)}");
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.Slot)}");
        AddInclude($"{nameof(Cart.CartServiceRequests)}.{nameof(CartServiceRequest.providerDistrict)}.{nameof(ProviderDistrict.District)}");

    }

}