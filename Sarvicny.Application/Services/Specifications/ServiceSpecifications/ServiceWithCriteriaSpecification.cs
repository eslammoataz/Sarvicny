using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;



public class ServiceWithCriteriaSpecification : BaseSpecifications<Service>
{
    
    
    public ServiceWithCriteriaSpecification()
    {
        Includes.Add(s => s.Criteria);
    }
    
    
    public ServiceWithCriteriaSpecification(string serviceId) : base(s => s.ServiceID == serviceId)
    {
        Includes.Add(s => s.Criteria);
    }


}