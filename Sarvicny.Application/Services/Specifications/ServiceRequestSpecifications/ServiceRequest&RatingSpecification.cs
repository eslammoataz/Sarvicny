using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.ServiceRequestSpecifications
{
    public class ServiceRequest_RatingSpecification : BaseSpecifications<ServiceRequest>
    {
        public ServiceRequest_RatingSpecification()
        {
            AddInclude($"{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
            AddInclude($"{nameof(ServiceRequest.order)}.{nameof(Order.Customer)}");
        }
        public ServiceRequest_RatingSpecification(string serviceRequestId) : base(x => x.ServiceRequestID == serviceRequestId)
        {
            AddInclude($"{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
            AddInclude($"{nameof(ServiceRequest.order)}.{nameof(Order.Customer)}");
        }
    }
}
