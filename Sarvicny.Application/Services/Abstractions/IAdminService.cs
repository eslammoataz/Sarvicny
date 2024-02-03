﻿using Sarvicny.Contracts;
using Sarvicny.Domain.Entities.Users.ServicProviders;


namespace Sarvicny.Application.Services.Abstractions;

public interface IAdminService
{
    Task<Response<ICollection<object>>> GetAllCustomers();
    Task<Response<ICollection<object>>> GetAllServiceProviders();
    Task<Response<ICollection<object>>> GetAllServices();
    Task<Response<Provider>> ApproveServiceProviderRegister(string workerId);
    Task<Response<Provider>> RejectServiceProviderRegister(string workerId);
    Task<Response<List<object>>> GetServiceProvidersRegistrationRequests();
    Task<Response<List<object>>> getAllOrders();
    Task<Response<List<object>>> getAllRequestedOrders();
    Task<Response<List<object>>> getAllApprovededOrders();
    Task<Response<bool>> BlockServiceProvider(string workerId);
    Task<Response<bool>> UnBlockServiceProvider(string workerId);
}