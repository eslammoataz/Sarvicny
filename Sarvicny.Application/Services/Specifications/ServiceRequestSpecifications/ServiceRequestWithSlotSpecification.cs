using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.ServiceRequestSpecifications
{
    public class ServiceRequestWithSlotSpecification : BaseSpecifications<ServiceRequest>
    {
        public ServiceRequestWithSlotSpecification()
        {
            Includes.Add(s => s.Slot);
        }

        public ServiceRequestWithSlotSpecification(string requestId):base(s=>s.ServiceRequestID==requestId)
        {
            AddInclude($"{nameof(ServiceRequest.Slot)}");
        }


    }
}
