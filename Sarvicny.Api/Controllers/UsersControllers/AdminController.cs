using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services;
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


    [HttpGet("GetServiceProvidersRegisterationRequests")]
    public async Task<IActionResult> GetServiceProvidersRegistrationRequests()
    {
        var response = await _adminService.GetProvidersAddtionalServiceRequests();

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
        var response = await _adminService.ApproveProviderRegisteration(WorkerID);
        if (response.isError == true)
        {
            return BadRequest(response.isError);
        }
        else return Ok(response);


    }


    [HttpPost]
    [Route("RejectServiceProvider")]
    public async Task<IActionResult> RejectServiceProviderRegister(string workerId)
    {
        var response = await _adminService.RejectProviderRegisteration(workerId);
        if (response.isError == true)
        {
            return BadRequest(response.isError);
        }
        else return Ok(response);
    }


    [HttpGet("GetProvidersServiceRegisterationRequests")]
    public async Task<IActionResult> GetServiceProvidersServiceRegistrationRequests()
    {
        var response = await _adminService.GetProvidersRegistrationRequests();

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
    [Route("ApproveServiceForProvider")]
    public async Task<IActionResult> ApproveServiceForProvider(string workerId, string providerServiceId)
    {
        var response = await _adminService.ApproveServiceForProvider(workerId,providerServiceId);
        if (response.isError )
        {
            return BadRequest(response);
        }
        else return Ok(response);
    }

    [HttpPost]
    [Route("RejectServiceForProvider")]
    public async Task<IActionResult> RejectServiceForProvider(string workerId, string providerServiceId)
    {
        var response = await _adminService.RejectServiceForProvider(workerId, providerServiceId);
        if (response.isError )
        {
            return BadRequest(response);
        }
        else return Ok(response);
    }


    [HttpGet("getAllOrders")]
    public async Task<IActionResult> GetAllOrders()
    {
        var response = await _adminService.getAllOrders();

        if (response.isError)
        {
            return BadRequest(response);
        }


        return Ok(response);

    }

    //[HttpGet("getAllApprovedOrders")]
    //public async Task<IActionResult> getAllApprovedOrders()
    //{
    //    var response = await _adminService.getAllApprovedOrders();

    //    if (response.isError)
    //    {
    //        return NotFound(response);
    //    }


    //    return Ok(response);

    //}

    [HttpGet("getAllRequestedOrders")]
    public async Task<IActionResult> getAllRequestedOrders()
    {
        var response = await _adminService.getAllPendingOrPaidOrders();

        if (response.isError)
        {
            return BadRequest(response);
        }
        return Ok(response);

    }
    [HttpGet("getAllCanceledOrdersByProvider")]
    public async Task<IActionResult> getAllCanceledOrdersByProvider()
    {
        var response = await _adminService.getAllCanceledByProviderOrders();

        if (response.isError)
        {
            return BadRequest(response);
        }


        return Ok(response);

    }

    //[HttpGet("getAllRejectedOrders")]
    //public async Task<IActionResult> getAllRejectedOrders()
    //{
    //    var response = await _adminService.getAllRejectedOrders();

    //    if (response.isError)
    //    {
    //        return NotFound(response);
    //    }


    //    return Ok(response);

    //}

    //[HttpGet("getAllExpiredOrders")]
    //public async Task<IActionResult> getAllExpiredOrders()
    //{
    //    var response = await _adminService.getAllExpiredOrders();

    //    if (response.isError)
    //    {
    //        return NotFound(response);
    //    }


    //    return Ok(response);

    //}



    [HttpGet("RemoveAllPaymentExpiredOrders")]
    public async Task<IActionResult> RemoveAllPaymentExpiredOrders()
    {
        var response = await _adminService.RemoveAllPaymentExpiredOrders();

        if (response.isError)
        {
            return NotFound(response);
        }


        return Ok(response);

    }
    [HttpGet("EnableSlotsForExpiredOrders")]
    public async Task<IActionResult> EnableSlotsForExpiredOrders()
    {
        var response = await _adminService.EnableSlotsForExpiredOrders();

        if (response.isError)
        {
            return NotFound(response);
        }


        return Ok(response);

    }

    [HttpPost]
    [Route("ReAssignOrder/{orderId}")]
    public async Task<IActionResult> ReAssignOrder(string orderId)
    {
        var response = await _adminService.ReAssignOrder(orderId);
        
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



    }


}
