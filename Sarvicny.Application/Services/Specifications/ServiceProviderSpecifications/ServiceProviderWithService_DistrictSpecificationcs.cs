
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications
{
    public class ServiceProviderWithService_DistrictSpecificationcs : BaseSpecifications<Provider>
    {
        public ServiceProviderWithService_DistrictSpecificationcs()
        {
            AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
            AddInclude($"{nameof(Provider.ProviderDistricts)}.{nameof(ProviderDistrict.District)}");
        }
        public ServiceProviderWithService_DistrictSpecificationcs(string providerId):base(p=>p.Id==providerId)
        {
            AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
            AddInclude($"{nameof(Provider.ProviderDistricts)}.{nameof(ProviderDistrict.District)}");
        }
    }
}
