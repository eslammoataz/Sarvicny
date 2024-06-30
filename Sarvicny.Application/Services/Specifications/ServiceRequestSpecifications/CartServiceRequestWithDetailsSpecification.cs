using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceRequestSpecifications;

public class CartServiceRequestWithDetailsSpecification : BaseSpecifications<CartServiceRequest>
{
    public CartServiceRequestWithDetailsSpecification()
    {

        AddInclude($"{nameof(CartServiceRequest.Cart)}.{nameof(Cart.Customer)}");
        AddInclude($"{nameof(CartServiceRequest.providerDistrict)}.{nameof(ProviderDistrict.District)}");
        AddInclude($"{nameof(CartServiceRequest.Provider)}");
        AddInclude($"{nameof(CartServiceRequest.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.ProviderServices)}");
        AddInclude($"{nameof(CartServiceRequest.Slot)}.{nameof(AvailabilityTimeSlot.ProviderAvailability)}");
        


    }
    public CartServiceRequestWithDetailsSpecification(string serviceRequestId) : base(x => x.CartServiceRequestID == serviceRequestId)
    {
        AddInclude($"{nameof(CartServiceRequest.Cart)}.{nameof(Cart.Customer)}");
        AddInclude($"{nameof(CartServiceRequest.providerDistrict)}.{nameof(ProviderDistrict.District)}");
        AddInclude($"{nameof(CartServiceRequest.Provider)}");
        AddInclude($"{nameof(CartServiceRequest.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.ProviderServices)}");
        AddInclude($"{nameof(CartServiceRequest.Slot)}.{nameof(AvailabilityTimeSlot.ProviderAvailability)}");
        

    }


}