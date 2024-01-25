using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.OrderSpecifications
{
    public class OrderWithCustomerSpecification : BaseSpecifications<Order>
    {
        public OrderWithCustomerSpecification() { 
            Includes.Add(o=>o.Customer);
            
        }
    }
}
