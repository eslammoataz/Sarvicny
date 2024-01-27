using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;

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


    [HttpGet("getRequests")]
    public async Task<IActionResult> GetRequests()
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
}