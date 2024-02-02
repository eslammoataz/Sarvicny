using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.OrderSpecifications
{
   public class OrderWithRequestsSpecification: BaseSpecifications<Order>
    {
        public OrderWithRequestsSpecification() {
            Includes.Add(o => o.OrderStatus);
            Includes.Add(o => o.Customer);

            AddInclude($"{nameof(Order.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
            AddInclude($"{nameof(Order.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(Order.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");          
            AddInclude($"{nameof(Order.ServiceRequests)}.{nameof(ServiceRequest.Slot)}");

        }
        public OrderWithRequestsSpecification(string orderId):base(o=>o.OrderID==orderId)
        {
            Includes.Add(o => o.OrderStatus);
            Includes.Add(o => o.Customer);
            AddInclude($"{nameof(Order.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
            AddInclude($"{nameof(Order.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(Order.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
            AddInclude($"{nameof(Order.ServiceRequests)}.{nameof(ServiceRequest.Slot)}");
        }
    }

}
