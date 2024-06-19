using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.ServiceRequestSpecifications
{
    public class ServiceRequest_RatingSpecification : BaseSpecifications<OrderServiceRequest>
    {
        public ServiceRequest_RatingSpecification()
        {
            AddInclude($"{nameof(OrderServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
            AddInclude($"{nameof(OrderServiceRequest.Order)}.{nameof(Order.Customer)}");
        }
        public ServiceRequest_RatingSpecification(string serviceRequestId) : base(x => x.OrderServiceRequestID == serviceRequestId)
        {
            AddInclude($"{nameof(OrderServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
            AddInclude($"{nameof(OrderServiceRequest.Order)}.{nameof(Order.Customer)}");
        }
    }
}
