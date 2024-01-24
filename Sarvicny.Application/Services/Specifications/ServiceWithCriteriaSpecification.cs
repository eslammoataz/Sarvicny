using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications;

public class ServiceWithCriteriaSpecification : BaseSpecifications<Service>
{
    public ServiceWithCriteriaSpecification()
    {
        Includes.Add(s => s.Criteria);
    }
    
    
}