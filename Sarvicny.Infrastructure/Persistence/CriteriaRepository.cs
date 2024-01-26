using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Infrastructure.Persistence;

public class CriteriaRepository : ICriteriaRepository
{
    private readonly AppDbContext _context;

    public CriteriaRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Criteria> GetCriteriaById(string criteriaId)
    {
        return await _context.Criterias.FindAsync(criteriaId);
    }

    public async Task<ICollection<Criteria>> GetAllCriterias()
    {
        return await _context.Criterias.ToListAsync();
    }

    public async Task<Criteria> GetCriteriaById(ISpecifications<Criteria> specifications)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Criteria>> GetAllCriterias(ISpecifications<Criteria> specifications)
    {
        throw new NotImplementedException();
    }

    public async Task<Criteria> UpdateCriteria(Criteria criteria)
    {
        throw new NotImplementedException();
    }

    public async Task<Criteria> DeleteCriteria(string criteriaId)
    {
        throw new NotImplementedException();
    }

    public async Task AddCriteriaAsync(Criteria newCriteria)
    {
        await _context.Criterias.AddAsync(newCriteria);
        
    }

    public async Task AddServiceToCriteria(string criteriaId, string serviceId)
    {
        var criteria = await _context.Criterias.FindAsync(criteriaId);
        var service = await _context.Services.FindAsync(serviceId);

        criteria.Services.Add(service);
    }
}