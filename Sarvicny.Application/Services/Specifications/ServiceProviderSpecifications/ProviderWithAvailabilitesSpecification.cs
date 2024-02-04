using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications
{
    public class ProviderWithAvailabilitesSpecification : BaseSpecifications<Provider>
    {
        public ProviderWithAvailabilitesSpecification()
        {


            Includes.Add(p => p.Availabilities);
        }
        public ProviderWithAvailabilitesSpecification(string providerId) : base(p => p.Id == providerId)
        {


            Includes.Add(p => p.Availabilities);
        }
    }
}
