using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.NewFolder1
{
    public class RatingWithCustomer_ProviderSpecification : BaseSpecifications<ServiceRequest>

    {
        AddInclude($"{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
        AddInclude($"{nameof(ServiceRequest.order)}.{nameof(Order.Customer)}");
    }
}
