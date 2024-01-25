using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;



public class ServiceWithCriteriaSpecification : BaseSpecifications<Service>
{
    public ServiceWithCriteriaSpecification()
    {
        Includes.Add(s => s.Criteria);
    }


}