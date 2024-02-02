
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications
{
    public class ServiceProviderWithServiceSpecificationcs : BaseSpecifications<Provider>
    {
        public ServiceProviderWithServiceSpecificationcs()
        {
            AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");

        }
        public ServiceProviderWithServiceSpecificationcs(string providerId):base(p=>p.Id==providerId)
        {
            AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.ParentService)}");
            AddInclude($"{nameof(Provider.ProviderServices)}.{nameof(ProviderService.Service)}.{nameof(Service.Criteria)}");
        }
    }
}
