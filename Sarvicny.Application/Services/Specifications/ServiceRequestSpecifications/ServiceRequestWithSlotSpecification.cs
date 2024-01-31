using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceRequestSpecifications;

public class ServiceRequestWithSlotSpecification : BaseSpecifications<ServiceRequest>
{
    public ServiceRequestWithSlotSpecification()
    {
        AddInclude(x => x.providerService);
    
    }
    public ServiceRequestWithSlotSpecification(string serviceRequestId) : base(x => x.ServiceRequestID == serviceRequestId)
    {
        AddInclude(x => x.providerService);
        AddInclude(x => x.Slot);
    }
    
    
}