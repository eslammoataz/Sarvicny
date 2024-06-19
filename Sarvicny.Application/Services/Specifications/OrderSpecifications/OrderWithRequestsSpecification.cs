using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.OrderSpecifications
{
    public class OrderWithRequestsSpecification : BaseSpecifications<Order>
    {
        public OrderWithRequestsSpecification()
        {
            Includes.Add(o => o.Customer);

            AddInclude($"{nameof(Order.OrderRequests)}.{nameof(OrderServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
            AddInclude($"{nameof(Order.OrderRequests)}.{nameof(OrderServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(Order.OrderRequests)}.{nameof(OrderServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
            AddInclude($"{nameof(Order.OrderRequests)}.{nameof(OrderServiceRequest.RequestedSlot)}");
            AddInclude($"{nameof(Order.OrderRequests)}.{nameof(OrderServiceRequest.providerDistrict)}.{nameof(ProviderDistrict.District)}");

        }
        public OrderWithRequestsSpecification(string orderId) : base(o => o.OrderID == orderId)
        {
            Includes.Add(o => o.Customer);
            AddInclude($"{nameof(Order.OrderRequests)}.{nameof(OrderServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
            AddInclude($"{nameof(Order.OrderRequests)}.{nameof(OrderServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(Order.OrderRequests)}.{nameof(OrderServiceRequest.providerService)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
            AddInclude($"{nameof(Order.OrderRequests)}.{nameof(OrderServiceRequest.RequestedSlot)}");
            AddInclude($"{nameof(Order.OrderRequests)}.{nameof(OrderServiceRequest.providerDistrict)}.{nameof(ProviderDistrict.District)}");
        }
    }

}
