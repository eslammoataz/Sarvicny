using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.OrderSpecifications
{

    public class OrderWithCustomers_Carts : BaseSpecifications<Order>
    {
        public OrderWithCustomers_Carts()
        {

            Includes.Add(o => o.Customer);
            Includes.Add(o => o.Customer.Cart);
            Includes.Add(o => o.Customer.Cart.ServiceRequests);

            //Includes.Add(o => o.Customer.Cart.ServiceRequests.providerService);

        }
    }
}
