using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts.Criteria;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = "Admin")]
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
    public async Task<IActionResult> GetCriteria(string id)
    {
        var criteria = await _criteriaService.GetCriteriaById(id);

        if (criteria is null)
        {
            return NotFound();
        }

        return Ok(criteria);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllCriterias()
    {
        var response = await _criteriaService.GetAllCriterias();

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }


    [HttpPost]
    [Route("addservice")]
    public async Task<IActionResult> AddServicesToCriteria(string criteriaId, string serviceId)
    {
        var response = await _criteriaService.AddServiceToCriteria(criteriaId, serviceId);


        if (response.isError)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }


}