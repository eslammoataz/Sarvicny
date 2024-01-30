using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.OrderSpecifications
{
    public class OrderWithProviderServiceSpecification : BaseSpecifications<Order>
    {
        public OrderWithProviderServiceSpecification() {
            Includes.Add(o => o.OrderStatus);
            Includes.Add(o => o.Customer);
            Includes.Add(o => o.Customer.Cart);
            Includes.Add(o => o.Customer.Cart.ServiceRequests);
            AddInclude($"{nameof(Order.Customer)}.{nameof(Customer.Cart)}.{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
            AddInclude($"{nameof(Order.Customer)}.{nameof(Customer.Cart)}.{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Service)}");
        }
        public OrderWithProviderServiceSpecification(string orderId) : base(o => o.OrderID == orderId)
        {
            Includes.Add(o => o.OrderStatus);
            Includes.Add(o => o.Customer);
            Includes.Add(o => o.Customer.Cart);
            Includes.Add(o => o.Customer.Cart.ServiceRequests);
            AddInclude($"{nameof(Order.Customer)}.{nameof(Customer.Cart)}.{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Provider)}");
            AddInclude($"{nameof(Order.Customer)}.{nameof(Customer.Cart)}.{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}.{nameof(ProviderService.Service)}");



        }
    }
}
