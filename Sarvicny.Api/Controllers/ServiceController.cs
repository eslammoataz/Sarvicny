using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts.Service;
using Sarvicny.Domain.Entities;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Api.Controllers;

[ApiController]
[Route("api/services")]
public class ServiceController : ControllerBase
{
    private readonly IServicesService _servicesService;
    private readonly ICriteriaService _criteriaService;

    public ServiceController(IServicesService servicesService, ICriteriaService criteriaService)
    {
        _servicesService = servicesService;
        _criteriaService = criteriaService;
    }
    
    [HttpGet]
    [Route("GetAllServices")]
    public async Task<IActionResult> GetAllServices()
    {
        var response = await _servicesService.GetAllServices();
        return Ok(response);
    }
    
    [HttpGet]
    [Route("GetServiceById")]
    public async Task<IActionResult> GetServiceById(string serviceId)
    {
        var response = await _servicesService.GetServiceById(serviceId);
        return Ok(response);
    }
    
    [HttpGet]
    [Route("GetAllWorkersForService")]
    public async Task<IActionResult> GetAllWorkersForService(string serviceId)
    {
        var response = await _servicesService.GetAllWorkersForService(serviceId);
        return Ok(response);
    }
    
    [HttpPost]
    [Route("AddService")]
    public async Task<IActionResult> AddService([FromBody] AddServiceDto serviceDto)
    {
        // Map DTO to your entity model
        var newService = new Service
        {
            ServiceName = serviceDto.ServiceName,
            Description = serviceDto.Description,
            Price = serviceDto.Price,
            AvailabilityStatus = serviceDto.AvailabilityStatus,
        };

        var response = await _servicesService.AddServiceAsync(newService);
        if (response.isError)
        {
            return BadRequest(response);
        }

        return Ok(newService);
    }
}