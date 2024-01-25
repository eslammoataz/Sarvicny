using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications
{
    public class ProviderWithAvailabilitesSpecification : BaseSpecifications<Provider>
    {
        public ProviderWithAvailabilitesSpecification() {

            
            Includes.Add(p => p.Availabilities.SelectMany(a => a.Slots));
        }
    }
}
