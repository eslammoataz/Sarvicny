using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Email;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities;
using Sarvicny.Infrastructure.Data;
using Sarvicny.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Microsoft.AspNetCore.Authentication;
using Sarvicny.Application.Services.Abstractions;
using IAuthenticationService = Sarvicny.Application.Services.Abstractions.IAuthenticationService;




namespace Sarvicny.Api.Controllers.UsersControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IConfiguration config;
        private readonly UserManager<User> workerManager;
        private readonly ILogger<ServiceProviderController> logger;
        private readonly ServiceProviderService serviceProviderService;
        private readonly IEmailService emailService;
        private readonly IServiceProviderService workerServices;
        private readonly IAuthenticationService authenticationService;
        private readonly RoleManager<IdentityRole> roleManager;
        public ServiceProviderController(AppDbContext _context, IConfiguration _config,
        RoleManager<IdentityRole> _roleManager, UserManager<User> _customerManager,
        ILogger<ServiceProviderController> logger, ServiceProviderService serviceProviderService, IEmailService emailService, IAuthenticationService authenticationService, IServiceProviderService workerServices)
        {
            context = _context;
            config = _config;
            roleManager = _roleManager;
            workerManager = _customerManager;
            this.logger = logger;
            this.serviceProviderService = serviceProviderService;
            this.emailService = emailService;
            this.workerServices = workerServices;
        }


        [HttpPost]
        [Route("SetAvailability")]
        public async Task<IActionResult> AddAvailability(AvailabilityDto availabilityDto, string workerID)
        {

            var Response = await serviceProviderService.AddAvailability(availabilityDto, workerID);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);

        }


        [HttpPost]
        [Route("SetAvailabilitySlots")]
        public async Task<IActionResult> AddAvailabilitySlots(TimeSlotDto slotDto, string availabilityId)
        {
            var Response = await serviceProviderService.AddAvailabilitySlots(slotDto, availabilityId);
            if (Response.isError) 
            { 
                return BadRequest(Response);
            }
            return Ok(Response);

        }

        [HttpGet("GetServiceProviderAvailability/{providerId}")]
        public async Task<IActionResult> GetServiceProviderAvailabilityAsync(string providerId)

        {
            var Response = await serviceProviderService.getAvailability(providerId);

            if (Response == null)
            {
                return Ok("There is no availability for this worker");
            }

            return Ok(Response);
        }


       


        
        [HttpPost]
        [Route("approveorder")]
        public async Task<IActionResult> ApproveOrder(string orderId)
        {
            var Response = await serviceProviderService.ApproveOrder(orderId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);
        }

        [HttpPost]
        [Route("rejectorder")]
        public async Task<IActionResult> RejectOrder(string orderId)
        {
            var Response = await serviceProviderService.RejectOrder(orderId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);
        }

        [HttpPost]
        [Route("Cancelorder")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            var Response = await serviceProviderService.CancelOrder(orderId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);
        }


        [HttpGet("ShowOrders")]
        public async Task<IActionResult> ShowOrderDetails(string orderId)
        {
            var Response = await serviceProviderService.ShowOrderDetails(orderId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);
        }

        [HttpGet("GetProviders")]
        public async Task<IActionResult> GetAllServiceProviders()
        {
            var Response = await serviceProviderService.GetAllServiceProviders();

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);
        }


    }
}

