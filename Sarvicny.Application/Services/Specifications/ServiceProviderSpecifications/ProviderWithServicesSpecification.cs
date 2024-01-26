using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;

public class ProviderWithServicesSpecification : BaseSpecifications<Provider>
{
    public ProviderWithServicesSpecification()
    {
        Includes.Add(p => p.ProviderServices);
    }
    
    public ProviderWithServicesSpecification(string providerId): base(p => p.Id == providerId)
    {
        Includes.Add(p => p.ProviderServices);
    }
}