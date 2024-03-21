using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.CustomerSpecification
{
    public class CustomerWithOrdersSpecification : BaseSpecifications<Customer>
    {
        public CustomerWithOrdersSpecification()
        {

            AddInclude($"{nameof(Customer.Orders)}.{nameof(Order.OrderStatus)}");

        }
        public CustomerWithOrdersSpecification(string customerID) : base(c => c.Id == customerID)
        {

            AddInclude($"{nameof(Customer.Orders)}.{nameof(Order.OrderStatus)}");
        }
    }
}
