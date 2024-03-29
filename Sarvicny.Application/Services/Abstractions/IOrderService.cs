﻿using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IOrderService
    {
        Task<Response<object>> ShowOrderDetailsForProvider(string orderId);
        Task<Response<object>> ShowAllOrderDetails(string orderId);
        Task<Response<object>> ShowAllOrderDetailsForCustomer(string orderId);
        Task<Response<object>> ShowOrderStatus(string orderId);
        Task<Response<object>> AddCustomerRating(CustomerRating customerRating);
        Task<Response<object>> AddProviderRating(ProviderRating providerRating);
     
    }
}
