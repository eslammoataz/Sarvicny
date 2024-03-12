using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Api.Controllers.UsersControllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }


    [HttpGet("getCustomers")]
    public async Task<IActionResult> GetAllCustomers()
    {
        return Ok(await _adminService.GetAllCustomers());
    }


    [HttpGet("getServiceProviders")]
    public async Task<IActionResult> GetAllServiceProviders()
    {
        return Ok(await _adminService.GetAllServiceProviders());
    }


    [HttpGet("getServices")]
    public async Task<IActionResult> GetAllServices()
    {
        return Ok(await _adminService.GetAllServices());
    }


    [HttpGet("GetServiceProvidersRegistrationRequests")]
    public async Task<IActionResult> GetServiceProvidersRegistrationRequests()
    {
        var response = await _adminService.GetServiceProvidersRegistrationRequests();

        if (response.isError)
        {
            return NotFound(response);
        }
        else
        {
            return Ok(response);
        }
    }


    [HttpPost]
    [Route("ApproveServiceProvider")]
    public async Task<IActionResult> ApproveServiceProviderRegister(string WorkerID)
    {
        var response = await _adminService.ApproveServiceProviderRegister(WorkerID);
        if (response.Payload == null)
        {
            return NotFound(response.Message);
        }
        else return Ok(response);


    }


    [HttpPost]
    [Route("RejectServiceProvider")]
    public async Task<IActionResult> RejectServiceProviderRegister(string workerId)
    {
        var response = await _adminService.RejectServiceProviderRegister(workerId);
        if (response.Payload == null)
        {
            return NotFound(response.Message);
        }
        else return Ok(response);
    }

    [HttpGet("getAllOrders")]
    public async Task<IActionResult> GetAllOrders()
    {
        var response = await _adminService.getAllOrders();

        if (response.isError)
        {
            return NotFound(response);
        }


        return Ok(response);

    }

    [HttpGet("getAllApprovedOrders")]
    public async Task<IActionResult> getAllApprovedOrders()
    {
        var response = await _adminService.getAllApprovededOrders();

        if (response.isError)
        {
            return NotFound(response);
        }


        return Ok(response);

    }

    [HttpGet("getAllRequestedOrders")]
    public async Task<IActionResult> getAllRequestedOrders()
    {
        var response = await _adminService.getAllRequestedOrders();

        if (response.isError)
        {
            return NotFound(response);
        }
        return Ok(response);

    }

    [HttpPost("BlockServiceProvider")]
    public async Task<IActionResult> BlockServiceProvider(string workerId)
    {
        var response = await _adminService.BlockServiceProvider(workerId);
        if (response.isError)
        {
            return NotFound(response);
        }
        return Ok(response);
    }

    [HttpPost("UnBlockServiceProvider")]
    public async Task<IActionResult> UnBlockServiceProvider(string workerId)
    {
        var response = await _adminService.UnBlockServiceProvider(workerId);
        if (response.isError)
        {
            return NotFound(response);
        }
        return Ok(response);
        //}

        //[HttpPost]
        //[Route("AddDistrict")]
        //public async Task<IActionResult> AddDistrict(string districtName)
        //{
        //    var district = new District()
        //    {
        //        DistrictName = districtName,
        //        Availability = true

        //    };
        //    var response = await _adminService.AddDistrict(district);
        //    if (response.Payload == null)
        //    {
        //        return NotFound(response.Message);
        //    }
        //    else return Ok(response);


        //}


    }

}
