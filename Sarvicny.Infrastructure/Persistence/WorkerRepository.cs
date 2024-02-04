using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Infrastructure.Persistence
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly AppDbContext _context;
        public WorkerRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Worker?> FindByIdAsync(ISpecifications<Worker> specifications)
        {
            return await ApplySpecification(specifications).FirstOrDefaultAsync();
        }

        private IQueryable<Worker> ApplySpecification(ISpecifications<Worker> spec)
        {
            return SpecificationBuilder<Worker>.Build(_context.Workers, spec);
        }
    }
}
