using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.OrderSpecifications
{
    public class OrderWithDetailsSpecification : BaseSpecifications<Order>
    {
        public OrderWithDetailsSpecification()
        {
            Includes.Add(o => o.Customer);

            AddInclude($"{nameof(Order.OrderDetails)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.Provider)}");
            AddInclude($"{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedServices)}.{nameof(RequestedService.Services)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedServices)}.{nameof(RequestedService.Services)}.{nameof(Service.Criteria)}");
            AddInclude($"{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedSlot)}");
            AddInclude($"{nameof(Order.OrderDetails)}.{nameof(OrderDetails.providerDistrict)}.{nameof(ProviderDistrict.District)}");
            AddInclude($"{nameof(Order.CRate)}");
            AddInclude($"{nameof(Order.PRate)}");

        }
        public OrderWithDetailsSpecification(string orderId) : base(o => o.OrderID == orderId)
        {
            Includes.Add(o => o.Customer);
            AddInclude($"{nameof(Order.OrderDetails)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.Provider)}");
            AddInclude($"{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedServices)}.{nameof(RequestedService.Services)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedServices)}.{nameof(RequestedService.Services)}.{nameof(Service.Criteria)}");
            AddInclude($"{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedSlot)}");
            AddInclude($"{nameof(Order.OrderDetails)}.{nameof(OrderDetails.providerDistrict)}.{nameof(ProviderDistrict.District)}");
            AddInclude($"{nameof(Order.CRate)}");
            AddInclude($"{nameof(Order.PRate)}");
        }
    }

}
