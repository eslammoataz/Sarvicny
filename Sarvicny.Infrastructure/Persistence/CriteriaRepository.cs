using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;
using System.Text.Json.Serialization;

namespace Sarvicny.Infrastructure.Persistence;



public class CriteriaRepository : ICriteriaRepository
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork ;

    public CriteriaRepository(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    private IQueryable<Criteria> ApplySpecification(ISpecifications<Criteria> spec)
    {
        return SpecificationBuilder<Criteria>.Build(_context.Criterias, spec);
    }



    public async Task<ICollection<Criteria>> GetAllCriterias(ISpecifications<Criteria> spec)
    {
        
        var criterias= await ApplySpecification(spec).ToListAsync();
     
        foreach(var criteria in criterias)
        {
            var services = criteria.Services.Select(s => new
            {
                s.ServiceID,
                s.ServiceName,
                s.Description,
                s.AvailabilityStatus,
                s.ParentServiceID,
                s.Price
            }).ToList();

            //assign
            criteria.Services = services.Select(s => new Service
            {
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName,
                Description = s.Description,
                AvailabilityStatus = s.AvailabilityStatus,
                ParentServiceID = s.ParentServiceID,
                Price = s.Price
            }).ToList();
        }
        return criterias;
    }

    public async Task<Criteria> GetCriteriaById(string criteriaId)
    {
        return await _context.Criterias.FindAsync(criteriaId);
    }
    public async Task<Criteria> GetCriteriaById(ISpecifications<Criteria> specifications)
    {
        var criteria= await ApplySpecification(specifications).FirstOrDefaultAsync();

        criteria.Services= criteria.Services.Select(s => new Service
        {
            ServiceID = s.ServiceID,
            ServiceName = s.ServiceName,
            Description = s.Description,
            AvailabilityStatus = s.AvailabilityStatus,
            ParentServiceID = s.ParentServiceID,
            Price = s.Price
        }).ToList();

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
        _unitOfWork.Commit();
        return newCriteria;

    }

    public async Task<Criteria> AddServiceToCriteria(string criteriaId,string serviceId)
    {
        var criteria = await _context.Criterias.FindAsync(criteriaId);
        var service = await _context.Services.FindAsync(serviceId);

        criteria.Services.Add(service);
        service.Criteria=(criteria);
        service.Criteria.CriteriaID = criteriaId;
        _unitOfWork.Commit();
        return criteria;
    }


}