﻿using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Contracts.Service;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Api.Controllers;

[ApiController]
[Route("api/services")]
public class ServiceController : ControllerBase
{
    private readonly IServicesService _servicesService;
    private readonly ICriteriaService _criteriaService;
    private readonly IOrderService _orderService;

    public ServiceController(IServicesService servicesService, ICriteriaService criteriaService, IOrderService orderService)
    {
        _servicesService = servicesService;
        _criteriaService = criteriaService;
        _orderService = orderService;
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

        if (response.isError)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet]
    [Route("GetAllParentServices")]
    public async Task<IActionResult> GetAllParentServices()
    {
        var response = await _servicesService.GetAllParentServices();

        if (response.isError)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet]
    [Route("GetAllChildForService")]
    public async Task<IActionResult> GetAllChildsForService(string serviceId)
    {
        var response = await _servicesService.GetAllChildsForService(serviceId);

        if (response.isError)
        {
            return BadRequest(response);
        }

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
            ParentServiceID=serviceDto.ParentServiceID
        };

        var response = await _servicesService.AddServiceAsync(newService);
        
        if (response.isError)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
   
    
}