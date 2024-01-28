using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.NewFolder
{
    public class CriteriaWithServicesSpecification : BaseSpecifications<Criteria>
    {
        public CriteriaWithServicesSpecification()
        {
            Includes.Add(s => s.Services);

        }
        public CriteriaWithServicesSpecification(string criteriaId) : base(c => c.CriteriaID == criteriaId)
        {
            Includes.Add(s => s.Services);

        }
    }
}
