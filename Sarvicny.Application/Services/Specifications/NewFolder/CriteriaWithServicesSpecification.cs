using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Specifications.NewFolder
{
    public class CriteriaWithServicesSpecification: BaseSpecifications<Criteria>
    {
        public CriteriaWithServicesSpecification()
        {
            Includes.Add(s => s.Services);
            
        }
        public CriteriaWithServicesSpecification(string criteriaId): base(c=>c.CriteriaID==criteriaId)
        {
            Includes.Add(s => s.Services);

        }
    }
}
