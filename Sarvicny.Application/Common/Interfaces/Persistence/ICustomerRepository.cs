﻿using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface ICustomerRepository
    {
        Task<Cart> GetCart(ISpecifications<Cart> specifications);

        Task<Customer?> GetCustomerById(ISpecifications<Customer> specifications);

        Task<ServiceRequest> GetServiceRequestById(ISpecifications<ServiceRequest> spec);
        Task AddRequest(ServiceRequest newRequest);
        Task RemoveRequest(ServiceRequest specificRequest);
        Task EmptyCart(Cart cart);

        bool CreateCart(string customerID);
    }
}
