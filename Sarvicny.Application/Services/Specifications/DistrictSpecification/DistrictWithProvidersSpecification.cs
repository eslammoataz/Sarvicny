using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.DistrictSpecification
{
    public class DistrictWithProvidersSpecification :BaseSpecifications<District>
    {
       public DistrictWithProvidersSpecification() {

            AddInclude($"{nameof(District.ProviderDistricts)}.{nameof(ProviderDistrict.Provider)}");
        }
        public DistrictWithProvidersSpecification(string districtId) : base(d=>d.DistrictID==districtId)
        {

            AddInclude($"{nameof(District.ProviderDistricts)}.{nameof(ProviderDistrict.Provider)}");
        }
    }
}
