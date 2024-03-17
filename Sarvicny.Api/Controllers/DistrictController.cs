using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {

        private readonly IAdminService _adminService;
        private readonly IServiceProviderService _serviceProviderService;

       public DistrictController(IAdminService adminService, IServiceProviderService serviceProviderService)
        {
            _adminService = adminService;
            _serviceProviderService = serviceProviderService;
        }


        [HttpPost]
        [Route("AddDistrict")]
        public async Task<IActionResult> AddDistrict(string districtName)
        {
            var district = new District()
            {
                DistrictName = districtName,
                Availability = true

            };
            var response = await _adminService.AddDistrict(district);
            if (response.Payload == null)
            {
                return NotFound(response.Message);
            }
            else return Ok(response);
        }


        //[HttpPost]
        //[Route("RequestDistrict")]
        //public async Task<IActionResult> RequestNewDistrictToBeAdded(string districtName)
        //{
        //    var response = await _serviceProviderService.RequestNewDistrictToBeAdded(districtName);

        //    if (response.isError)
        //    {
        //        return BadRequest(response);
        //    }

        //    return Ok(response);
        //}

        //[HttpGet("getAllRequestedDistricts")]
        //public async Task<IActionResult> getAllRequestedDistricts()
        //{
        //    var response = await _adminService.GetAllRequestedDistricts();

        //    if (response.isError)
        //    {
        //        return NotFound(response);
        //    }
        //    return Ok(response);

        //}

        [HttpGet("getAllAvailableDistricts")]
        public async Task<IActionResult> getAllAvailableDistricts()
        {
            var response = await _adminService.GetAllAvailableDistricts();

            if (response.isError)
            {
                return NotFound(response);
            }
            return Ok(response);

        }

        [HttpGet("getProviderDistricts/{providerid}")]
        public async Task<IActionResult> GetProviderDistricts(string providerID)
        {
            var response = await _serviceProviderService.GetProviderDistricts(providerID);

            if (response.isError)
            {
                return NotFound(response);
            }


            return Ok(response);
        }
        [HttpPost]
        [Route("AddDistrict/{providerId}")]
        public async Task<IActionResult> AddDistrictToProvider(string providerId, string districtID)
        {
            var response = await _serviceProviderService.AddDistrictToProvider(providerId, districtID);

            if (response.isError)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


    }
}
