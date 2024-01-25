
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications
{
    public class ServiceProviderWithServiceSpecificationcs: BaseSpecifications<Provider>
    {
       public ServiceProviderWithServiceSpecificationcs()
        {
            Includes.Add(w => w.ProviderServices.Select(ps => ps.Service));

        }
    }
}
