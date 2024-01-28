using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.NewFolder
{
    public class AvailaibiltyWithSlotsSpecification : BaseSpecifications<ProviderAvailability>
    {
        public AvailaibiltyWithSlotsSpecification()
        {
            Includes.Add(a => a.Slots);
        }
        public AvailaibiltyWithSlotsSpecification(String providerId): base(a => a.ServiceProviderID == providerId)
        {
            Includes.Add(a => a.Slots);
        }
    }
}
