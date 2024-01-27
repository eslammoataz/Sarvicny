using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Api.Controllers.UsersControllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AdminController> _logger;
    private readonly IEmailService _emailService;
    private readonly RoleManager<IdentityRole> _roleManager;


    public AdminController(IAdminService adminService, AppDbContext context, UserManager<User> adminManager,
        RoleManager<IdentityRole> roleManager, IConfiguration config, ILogger<AdminController> logger,
        IEmailService emailService)
    {
        _adminService = adminService;
        _context = context;
        _userManager = adminManager;
        _roleManager = roleManager;
        _config = config;
        _logger = logger;
        _emailService = emailService;
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
    public IActionResult GetRequests()
    {
        var unHandeledProviders = _context.Provider.Where(w => w.isVerified == false).ToList();

        if (unHandeledProviders.Count == 0)
        {
            return NotFound("No Requests found");
        }
        else
        {
            return Ok(unHandeledProviders);
        }
    }


    [HttpPost]
    [Route("ApproveServiceProvider")]
    public async Task<IActionResult> ApproveServiceProviderRegister(string WorkerID)
    {
        var response = await _adminService.ApproveServiceProviderRegister(WorkerID);
        if (response.Payload == null) {
            return NotFound(response.Message);
        }
        else return Ok(response);

        
    }


    [HttpPost]
    [Route("RejectServiceProvider")]
    public async Task<IActionResult> RejectServiceProviderRegister(string workerId)
    {
        var response = await _adminService.ApproveServiceProviderRegister(workerId);
        if (response.Payload == null)
        {
            return NotFound(response.Message);
        }
        else return Ok(response);
    }
}