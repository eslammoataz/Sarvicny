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

    private IQueryable<Criteria> ApplySpecification(ISpecifications<Criteria> spec)
    {
        return SpecificationBuilder<Criteria>.Build(_context.Criterias, spec);
    }



    public async Task<ICollection<Criteria>> GetAllCriterias(ISpecifications<Criteria> spec)
    {
        var criterias = await ApplySpecification(spec).ToListAsync();
        return criterias;
    }


    public async Task<Criteria> GetCriteriaById(ISpecifications<Criteria> specifications)
    {
        var criteria = await ApplySpecification(specifications).FirstOrDefaultAsync();

        return criteria;
    }

    public async Task<Criteria> UpdateCriteria(Criteria criteria)
    {
        throw new NotImplementedException();
    }

    public async Task<Criteria> DeleteCriteria(string criteriaId)
    {
        throw new NotImplementedException();
    }

    public async Task<Criteria> AddCriteriaAsync(Criteria newCriteria)
    {
        await _context.Criterias.AddAsync(newCriteria);
       
        return newCriteria;

    }

    public async Task<Criteria> AddServiceToCriteria(string criteriaId, string serviceId)
    {
        var criteria = await _context.Criterias.FindAsync(criteriaId);
        var service = await _context.Services.FindAsync(serviceId);

        criteria.Services.Add(service);
        service.Criteria = (criteria);
        service.Criteria.CriteriaID = criteriaId;
      
        return criteria;
    }
}