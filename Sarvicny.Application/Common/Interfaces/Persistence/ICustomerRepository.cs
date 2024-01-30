using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface ICustomerRepository
    {
        Task<Cart> GetCart(ISpecifications<Cart> specifications);

        Task<Customer?> GetCustomerById(ISpecifications<Customer> specifications);
        Task AddRequest(ServiceRequest newRequest);
    }
}
