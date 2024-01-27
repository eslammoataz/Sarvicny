using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.ProviderServiceSpecifications;

public class ProviderServiceWithServiceSpecification : BaseSpecifications<ProviderService>
{
    ProviderServiceWithServiceSpecification()
    {
        Includes.Add(p => p.Service);
    }
}