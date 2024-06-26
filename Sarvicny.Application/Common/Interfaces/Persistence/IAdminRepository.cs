﻿using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IAdminRepository
    {
        Task<Provider> ApproveServiceProviderRegister(ISpecifications<Provider> spec);
        Task<Provider> RejectServiceProviderRegister(string providerId);

        Task ApproveProviderService(string providerServiceID);
        Task RejectProviderService(string providerServiceID);


    }
}
