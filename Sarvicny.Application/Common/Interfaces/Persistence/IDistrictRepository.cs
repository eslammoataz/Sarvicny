using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IDistrictRepository
    {
        public Task<District> GetDistrictById(string districtId);
       public Task<District> GetDistrictByName(string districtName);
       public Task<District> AddDistrict(District district);
        public Task<ProviderDistrict> AddDistrictToProvider(ProviderDistrict providerDistrict);
        public Task<List<District>> GetAllDistricts(ISpecifications<District> specifications);

       // public Task<Response<object>> RequestNewDistrictToBeAdded(string districtName);






    }
}
