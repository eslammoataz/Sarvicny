using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions;

public interface ICriteriaService
{
    Task<Response<ICollection<Criteria>>> GetAllCriterias();
    Task<Response<Criteria>> GetCriteriaById(string criteriaId);
    Task<Response<Criteria>> UpdateCriteria(Criteria criteria);
    Task<Response<Criteria>> DeleteCriteria(string criteriaId);
    Task<Response<Criteria>> AddCriteriaAsync(Criteria newCriteria);
    
    Task<Response<Criteria>> AddServiceToCriteria(string criteriaId, string serviceId);
    
}