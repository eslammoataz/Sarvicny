using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.CartSpecifications;

public class CartWithServiceRequestsSpecification : BaseSpecifications<Cart>
{
    public CartWithServiceRequestsSpecification()
    {
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.Slot)}");
    }

    public CartWithServiceRequestsSpecification(string cartId) : base(c => c.CartID == cartId)
    {
        
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
        AddInclude($"{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.Slot)}");

    }

}