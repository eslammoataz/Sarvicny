using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications
{
    public class ProviderWithDistrictsSpecification: BaseSpecifications<Provider>
    {

        public ProviderWithDistrictsSpecification()
        {
            AddInclude($"{nameof(Provider.ProviderDistricts)}.{nameof(ProviderDistrict.District)}");
        }
        public ProviderWithDistrictsSpecification(string providerId) : base(p => p.Id == providerId)
        {
            AddInclude($"{nameof(Provider.ProviderDistricts)}.{nameof(ProviderDistrict.District)}");
        }
    }
}
