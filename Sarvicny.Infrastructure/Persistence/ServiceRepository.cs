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


    public async Task<ICollection<Service>> GetAllServices(ISpecifications<Service> spec)
    {
        return await ApplySpecificationS(spec).ToListAsync();
    }
   

    public async Task<Service> GetServiceById(ISpecifications<Service> specifications)
    {
        return await ApplySpecificationS(specifications).FirstOrDefaultAsync();
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
        await _context.Services.AddAsync(newService);
    }


    private IQueryable<Service> ApplySpecificationS(ISpecifications<Service> spec)
    {
        return SpecificationBuilder<Service>.Build(_context.Services, spec);
    }

    private IQueryable<ProviderService> ApplySpecificationPS(ISpecifications<ProviderService> spec)
    {
        return SpecificationBuilder<ProviderService>.Build(_context.ProviderServices, spec);
    }

    public Task<ICollection<Provider>> GetServiceById(string serviceId)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Service>> GetAllParentServices(ISpecifications<Service> spec)
    {
        return await ApplySpecificationS(spec).Where(s=>s.ParentServiceID==null).ToListAsync();
    }

    public  async Task AddRequestedService(RequestedService requestedService)
    {
        await _context.RequestedServices.AddAsync(requestedService);
    }
}