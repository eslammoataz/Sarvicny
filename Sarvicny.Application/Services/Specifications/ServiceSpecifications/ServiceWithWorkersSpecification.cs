using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceSpecifications;

public class ServiceWithWorkersSpecification : BaseSpecifications<Service>
{
    public ServiceWithWorkersSpecification()
    {
        // Includes.Add(s => s.ProviderServices);
        Includes.Add(s => s.ProviderServices.Select(ps => ps.Provider));
    }
}