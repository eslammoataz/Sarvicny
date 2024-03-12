using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;

public class ProviderWithServices_Districts_AndAvailabilitiesSpecification : BaseSpecifications<Provider>
{
    public ProviderWithServices_Districts_AndAvailabilitiesSpecification()
    {
        AddInclude($"{nameof(Provider.Availabilities)}.{nameof(ProviderAvailability.Slots)}");
        AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
        AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
        AddInclude($"{nameof(Provider.ProviderDistricts)}.{nameof(ProviderDistrict.District)}");
    }

    public ProviderWithServices_Districts_AndAvailabilitiesSpecification(string providerId) : base(p => p.Id == providerId)
    {
        AddInclude($"{nameof(Provider.Availabilities)}.{nameof(ProviderAvailability.Slots)}");
        AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
        AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
        AddInclude($"{nameof(Provider.ProviderDistricts)}.{nameof(ProviderDistrict.District)}");
    }
}