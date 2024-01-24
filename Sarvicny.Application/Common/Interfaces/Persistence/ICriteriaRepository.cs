using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Common.Interfaces.Persistence;

public interface ICriteriaRepository
{
    Task<Criteria> GetCriteriaById(string criteriaId);
    Task<ICollection<Criteria>> GetAllCriterias();
    Task<Criteria> UpdateCriteria(Criteria criteria);
    Task<Criteria> DeleteCriteria(string criteriaId);
    Task AddCriteriaAsync(Criteria newCriteria);
    
    Task AddServiceToCriteria(string criteriaId, string serviceId);
}