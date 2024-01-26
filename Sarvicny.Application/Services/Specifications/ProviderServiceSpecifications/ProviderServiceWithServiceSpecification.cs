using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.ProviderServiceSpecifications;

public class ProviderServiceWithServiceSpecification : BaseSpecifications<ProviderService>
{
    ProviderServiceWithServiceSpecification()
    {
        Includes.Add(p => p.Service);
    }
}