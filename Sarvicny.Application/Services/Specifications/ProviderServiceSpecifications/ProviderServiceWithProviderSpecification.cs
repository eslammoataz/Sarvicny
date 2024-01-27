using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ProviderServiceSpecifications
{
    public class ProviderServiceWithProviderSpecification : BaseSpecifications<ProviderService>
    {
        public ProviderServiceWithProviderSpecification()
        {
            Includes.Add(p => p.Provider);

        }
        public ProviderServiceWithProviderSpecification(string ServiceId) : base(p => p.ServiceID == ServiceId)
        {
            Includes.Add(p => p.Provider);

        }

    }
}
