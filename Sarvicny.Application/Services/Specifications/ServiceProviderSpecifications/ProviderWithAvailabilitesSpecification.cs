using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications
{
    public class ProviderWithAvailabilitesSpecification : BaseSpecifications<Provider>
    {
        public ProviderWithAvailabilitesSpecification()
        {


            Includes.Add(p => p.Availabilities.SelectMany(a => a.Slots));
        }
    }
}
