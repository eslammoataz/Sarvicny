using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceSpecifications;

public class ServiceWithProvidersSpecification : BaseSpecifications<Service>
{
    public ServiceWithProvidersSpecification()
    {

        AddInclude($"{nameof(Service.ProviderServices)}.{nameof(ProviderService.Provider)}.{nameof(Provider.Availabilities)}.{nameof(ProviderAvailability.Slots)}");
    }

    public ServiceWithProvidersSpecification(string serviceId) : base(s => s.ServiceID == serviceId)
    {
        AddInclude($"{nameof(Service.ProviderServices)}.{nameof(ProviderService.Provider)}.{nameof(Provider.Availabilities)}.{nameof(ProviderAvailability.Slots)}");
    }

}