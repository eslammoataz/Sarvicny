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
        Task<Provider> ApproveServiceProviderRegister(string providerId);
        Task<Provider> RejectServiceProviderRegister(string providerId);
    }
}
