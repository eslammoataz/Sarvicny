using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.CustomerSpecification;

public class CustomerWithCartSpecification : BaseSpecifications<Customer>
{
    public CustomerWithCartSpecification()
    {
        Includes.Add(c => c.Cart);
        Includes.Add(c => c.Cart.ServiceRequests);
        AddInclude($"{nameof(Customer.Cart)}.{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerService)}");
        AddInclude($"{nameof(Customer.Cart)}.{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.Slot)}.{nameof(AvailabilityTimeSlot.ProviderAvailability)}");
    }

    public CustomerWithCartSpecification(string customerId) : base(c => c.Id == customerId)
    {
        Includes.Add(c => c.Cart);
        Includes.Add(c => c.Cart.ServiceRequests);
        AddInclude($"{nameof(Customer.Cart)}.{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.providerService)}");
        AddInclude($"{nameof(Customer.Cart)}.{nameof(Cart.ServiceRequests)}.{nameof(CartServiceRequest.Slot)}.{nameof(AvailabilityTimeSlot.ProviderAvailability)}");

    }

}