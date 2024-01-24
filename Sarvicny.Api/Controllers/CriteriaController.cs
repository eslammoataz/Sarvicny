﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts.Criteria;
using Sarvicny.Domain.Entities;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CriteriaController : ControllerBase
{
    private readonly ICriteriaService _criteriaService;

    public CriteriaController(ICriteriaService criteriaService)
    {
        _criteriaService = criteriaService;
    }

    [HttpPost]
    public async Task<IActionResult> AddCriteria([FromBody] AddCriteriaDto criteriaDto)
    {
        // Map DTO to your entity model
        var newCriteria = new Criteria
        {
            CriteriaName = criteriaDto.CriteriaName,
            Description = criteriaDto.Description,
        };

        await _criteriaService.AddCriteriaAsync(newCriteria);

        // Return the newly created service
        return CreatedAtAction(nameof(GetCriteria), new { id = newCriteria.CriteriaID }, newCriteria);
    }


    [HttpGet("{id}")]
    public IActionResult GetCriteria(string id)
    {
        var criteria = _criteriaService.GetCriteriaById(id);

        if (criteria is null)
        {
            return NotFound();
        }

        return Ok(criteria);
    }


    [HttpPost]
    [Route("addservice")]
    public async Task<IActionResult> AddServicesToCriteria(string criteriaId, [FromBody] string serviceId)
    {
        var response = await _criteriaService.AddServiceToCriteria(serviceId, criteriaId);


        if (response.isError)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}