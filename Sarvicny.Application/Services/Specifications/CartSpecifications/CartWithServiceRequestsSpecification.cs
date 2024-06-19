using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.CartSpecifications;

public class CartWithServiceRequestsSpecification : BaseSpecifications<Cart>
{
    public CartWithServiceRequestsSpecification()
    {
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerService)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.Slot)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerDistrict)}.{nameof(ProviderDistrict.District)}");
    }

    public CartWithServiceRequestsSpecification(string cartId) : base(c => c.CartID == cartId)
    {
        
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerService)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.Slot)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerDistrict)}.{nameof(ProviderDistrict.District)}");

    }

}