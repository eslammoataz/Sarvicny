using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.ServiceSpecifications
{
    public class ServiceWithParentAndChilds_CriteriaSpecification:BaseSpecifications<Service>
    {
         public ServiceWithParentAndChilds_CriteriaSpecification()
        {
            Includes.Add(s => s.Criteria);
            Includes.Add(s=>s.ParentService); 
            Includes.Add(s => s.ChildServices);
        }

        public ServiceWithParentAndChilds_CriteriaSpecification(string serviceid):base(s=>s.ServiceID==serviceid)
        {
            Includes.Add(s => s.ParentService);
            Includes.Add(s => s.ChildServices);
            Includes.Add(s => s.Criteria);
        }

    }
}
