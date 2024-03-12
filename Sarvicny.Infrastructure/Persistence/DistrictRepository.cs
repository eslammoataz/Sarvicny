using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Infrastructure.Persistence
{
    public class DistrictRepository : IDistrictRepository
    {
        private readonly AppDbContext _context;
        public DistrictRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<District> AddDistrict(District district)
        {
            await _context.Districts.AddAsync(district);
            return district;
        }
        public async Task<ProviderDistrict> AddDistrictToProvider(ProviderDistrict providerDistrict)
        {
            await _context.ProviderDistricts.AddAsync(providerDistrict);
            return providerDistrict;
        }

        public async Task<List<District>> GetAllDistricts()
        {
            return await _context.Districts.ToListAsync();
            
        }

        public async Task<District> GetDistrictById(string districtId)
        {
            return await _context.Districts.FirstOrDefaultAsync(d => d.DistrictID == districtId);
        }
        public async Task<District> GetDistrictByName(string districtName)
        {
            return await _context.Districts.FirstOrDefaultAsync(d => d.DistrictName == districtName);
        }

        public Task<Response<object>> RequestNewDistrictToBeAdded(string districtName)
        {
            throw new NotImplementedException();
        }
    }
}
