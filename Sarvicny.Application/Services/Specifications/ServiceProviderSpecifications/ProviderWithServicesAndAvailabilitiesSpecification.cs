using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;

public class ProviderWithServicesAndAvailabilitiesSpecification : BaseSpecifications<Provider>
{
    public ProviderWithServicesAndAvailabilitiesSpecification()
    {
        Includes.Add(p => p.ProviderServices);
        Includes.Add(p=>p.Availabilities);
    }

    public ProviderWithServicesAndAvailabilitiesSpecification(string providerId) : base(p => p.Id == providerId)
    {
        Includes.Add(p => p.ProviderServices);
        Includes.Add(p=>p.Availabilities);
        AddInclude($"{nameof(Provider.Availabilities)}.{nameof(ProviderAvailability.Slots)}");
    }
}