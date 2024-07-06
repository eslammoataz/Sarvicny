using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.OrderSpecifications
{
    public class TransactionPaymentWithDetailsSpecification : BaseSpecifications<TransactionPayment>
    {
        public TransactionPaymentWithDetailsSpecification() {


            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.Customer)}.{nameof(Customer.Cart)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.Provider)}.{nameof(Provider.Wallet)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.Criteria)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.ProviderServices)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedSlot)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.providerDistrict)}.{nameof(ProviderDistrict.District)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.CRate)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.PRate)}");

        }
        public TransactionPaymentWithDetailsSpecification(string transactionPaymentId) : base(t=>t.TransactionPaymentID == transactionPaymentId)
        {


            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.Customer)}.{nameof(Customer.Cart)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.Provider)}.{nameof(Provider.Wallet)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.Criteria)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedServices)}.{nameof(RequestedService.Service)}.{nameof(Service.ProviderServices)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.RequestedSlot)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.OrderDetails)}.{nameof(OrderDetails.providerDistrict)}.{nameof(ProviderDistrict.District)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.CRate)}");
            AddInclude($"{nameof(TransactionPayment.OrderList)}.{nameof(Order.PRate)}");

        }
    }
}
