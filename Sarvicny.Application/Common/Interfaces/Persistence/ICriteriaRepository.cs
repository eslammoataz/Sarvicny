using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence;

public interface ICriteriaRepository
{
    Task<Criteria> GetCriteriaById(ISpecifications<Criteria> specifications);
    Task<ICollection<Criteria>> GetAllCriterias(ISpecifications<Criteria> specifications);
    Task<Criteria> UpdateCriteria(Criteria criteria);
    Task<Criteria> DeleteCriteria(string criteriaId);
    Task AddCriteriaAsync(Criteria newCriteria);
    
    Task AddServiceToCriteria(string criteriaId, string serviceId);
}