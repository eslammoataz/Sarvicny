using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   //[Authorize(Roles = "ServiceProvider ,Admin")]
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

        [HttpGet("getProviderDistricts/{providerId}")]
        public async Task<IActionResult> GetProviderDistricts(string providerId)
        {
            var response = await _serviceProviderService.GetProviderDistricts(providerId);

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

        //[HttpPost]
        //[Route("RemoveDistrict/{providerId}")]
        //public async Task<IActionResult> RemoveDistrictFromProvider(string providerId, string districtID)
        //{
        //    var response = await _serviceProviderService.RemoveDistrictFromProvider(providerId, districtID);

        //    if (response.isError)
        //    {
        //        return BadRequest(response);
        //    }

        //    return Ok(response);
        //}

        [HttpPost]
        [Route("DisableDistrict/{providerId}")]
        public async Task<IActionResult> DisableDistrictFromProvider(string providerId, string districtID)
        {
            var response = await _serviceProviderService.DisableDistrictFromProvider(providerId, districtID);

            if (response.isError)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("EnableDistrict/{providerId}")]
        public async Task<IActionResult> EnableDistrictToProvider(string providerId, string districtID)
        {
            var response = await _serviceProviderService.EnableDistrictToProvider(providerId, districtID);

            if (response.isError)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


    }
}
