using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.OrderSpecifications
{

    public class OrderWithCustomers_Carts : BaseSpecifications<Order>
    {
        public OrderWithCustomers_Carts()
        {
            Includes.Add(o => o.OrderStatus);
            Includes.Add(o => o.Customer);
            Includes.Add(o => o.Customer.Cart);
            Includes.Add(o => o.Customer.Cart.ServiceRequests);
            AddInclude($"{nameof(Order.Customer)}.{nameof(Customer.Cart)}.{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}");





        }
        public OrderWithCustomers_Carts(string orderId) : base(o=> o.OrderID==orderId) 
        {
            Includes.Add(o => o.OrderStatus);
            Includes.Add(o => o.Customer);
            Includes.Add(o => o.Customer.Cart);
            Includes.Add(o => o.Customer.Cart.ServiceRequests);
            AddInclude($"{nameof(Order.Customer)}.{nameof(Customer.Cart)}.{nameof(Cart.ServiceRequests)}.{nameof(ServiceRequest.providerService)}");




        }
    }
}
