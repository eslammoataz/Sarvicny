using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Infrastructure.Persistence;

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;

    public ServiceRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<ICollection<Provider>> GetAllServiceProviders()
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Service>> GetAllServices(ISpecifications<Service> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<Service> GetServiceById(string serviceId, ISpecifications<Service> specifications)
    {
        return await ApplySpecification(specifications).FirstOrDefaultAsync(s => s.ServiceID == serviceId);
    }

    public async Task<Service> UpdateService(Service service)
    {
        throw new NotImplementedException();
    }

    public async Task<Service> DeleteService(string serviceId)
    {
        throw new NotImplementedException();
    }

    public async Task AddServiceAsync(Service newService)
    {
        throw new NotImplementedException();
    }

    private IQueryable<Service> ApplySpecification(ISpecifications<Service> spec)
    {
        return SpecificationBuilder<Service>.Build(_context.Services, spec);
    }
    
}