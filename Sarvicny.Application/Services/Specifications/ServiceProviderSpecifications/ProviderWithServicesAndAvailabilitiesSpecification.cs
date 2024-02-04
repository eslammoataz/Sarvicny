using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;

public class ProviderWithServicesAndAvailabilitiesSpecification : BaseSpecifications<Provider>
{
    public ProviderWithServicesAndAvailabilitiesSpecification()
    {
        AddInclude($"{nameof(Provider.Availabilities)}.{nameof(ProviderAvailability.Slots)}");
        AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
        AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
    }

    public ProviderWithServicesAndAvailabilitiesSpecification(string providerId) : base(p => p.Id == providerId)
    {
        AddInclude($"{nameof(Provider.Availabilities)}.{nameof(ProviderAvailability.Slots)}");
        AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
        AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
    }
}