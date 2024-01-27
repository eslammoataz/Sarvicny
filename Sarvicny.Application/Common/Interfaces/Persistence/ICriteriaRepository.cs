using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence;

public interface ICriteriaRepository
{
    Task<Criteria> GetCriteriaById(string criteriaId);
    Task<Criteria> GetCriteriaById(ISpecifications<Criteria> specifications);
    Task<ICollection<Criteria>> GetAllCriterias(ISpecifications<Criteria> specifications);
    Task<Criteria> UpdateCriteria(Criteria criteria);
    Task<Criteria> DeleteCriteria(string criteriaId);
    Task<Criteria> AddCriteriaAsync(Criteria newCriteria);
    
    Task<Criteria> AddServiceToCriteria(string criteriaId, string serviceId);
}