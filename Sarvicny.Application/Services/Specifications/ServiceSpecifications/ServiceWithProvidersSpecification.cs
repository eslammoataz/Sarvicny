using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System.Linq.Expressions;

namespace Sarvicny.Application.Services.Specifications.ServiceSpecifications;

public class ServiceWithProvidersSpecification : BaseSpecifications<Service>
{
    public ServiceWithProvidersSpecification()
    {

        AddInclude(x => x.ProviderServices);

    }
    
    public ServiceWithProvidersSpecification(string serviceId): base(s=> s.ServiceID == serviceId)
    {
        AddInclude(x => x.ProviderServices);
    }




}