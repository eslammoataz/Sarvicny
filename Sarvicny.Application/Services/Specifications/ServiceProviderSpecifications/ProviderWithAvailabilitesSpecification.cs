using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications
{
    public class ProviderWithAvailabilitesSpecification : BaseSpecifications<Provider>
    {
        public ProviderWithAvailabilitesSpecification()
        {

            AddInclude($"{nameof(Provider.Availabilities)}.{nameof(ProviderAvailability.Slots)}");
        }
        public ProviderWithAvailabilitesSpecification(string providerId) : base(p => p.Id == providerId)
        {
            AddInclude($"{nameof(Provider.Availabilities)}.{nameof(ProviderAvailability.Slots)}");
        }
    }
}
