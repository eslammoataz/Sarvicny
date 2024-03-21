//using Sarvicny.Domain.Entities;
//using Sarvicny.Domain.Specification;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Sarvicny.Application.Services.Specifications.OrderSpecifications
//{
//    public class OrderWithCustomerSpecification : BaseSpecifications<Order>
//    {
//        public OrderWithCustomerSpecification() { 
//            Includes.Add(o=>o.Customer);

//        }

//        public OrderWithCustomerSpecification(string orderId) : base(o => o.OrderID == orderId)
//        {
//            Includes.Add(o => o.Customer);
//        }
//    }
//}
