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
            var availability = context.ProviderAvailabilities.FirstOrDefault(a => a.ProviderAvailabilityID == availabilityId);

            if (availability == null)
            {
                return BadRequest("worker Id is not valid");
            }

            var slot = new TimeSlot
            {
                ProviderAvailabilityID = availability.ProviderAvailabilityID,
                StartTime = slotDto.StartTime,
                EndTime = slotDto.EndTime,
                enable = true,

            };


            context.Slots.Add(slot);
            return CreatedAtAction(nameof(GetServiceProviderSlots), new { id = availability.ProviderAvailabilityID }, availability);

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


        [HttpGet("availability/{id}")]
        public IActionResult GetServiceProviderSlots(string avalId)

        {
            var availability = context.ProviderAvailabilities.FirstOrDefault(a => a.ProviderAvailabilityID == avalId);
            if (availability == null)
            {
                return BadRequest("Availability Id is not valid");
            }

            var slots = context.Slots.Where(s => s.ProviderAvailabilityID == availability.ProviderAvailabilityID).ToList();

            if (slots == null)
            {
                return NotFound();
            }

            return Ok(slots);
        }


        //[HttpPost]
        //[Route("SetAvailabilityAsPrev")]
        //public IActionResult SetServiceProviderAvailabilityAsPrev(string providerId)

        //{
        //    var provider = context.Provider.FirstOrDefault(s => s.Id == providerId);
        //    if (provider == null)
        //    {
        //        return BadRequest("worker Id is not valid");
        //    }
        //    DateTime currentDate = DateTime.Now;
        //    DateTime previousWeekStart = currentDate.AddDays(-7);
        //    DateTime previousWeekEnd = currentDate.AddDays(-1);


        //    var previousWeekAvailability = context.ProviderAvailabilities
        //        .FirstOrDefault(pa => pa.AvailabilityDate >= previousWeekStart && pa.AvailabilityDate <= previousWeekEnd);

        //    if (previousWeekAvailability == null)
        //    {
        //        return BadRequest("ther is no data in the previous week");
        //    }

        //    var newavailability = new ProviderAvailability
        //    {
        //        ServiceProviderID = provider.Id,
        //        DayOfWeek = previousWeekAvailability.DayOfWeek,
        //        AvailabilityDate = currentDate,
        //        ServiceProvider = provider,
        //        Slots = previousWeekAvailability.Slots

        //    };

        //    context.ProviderAvailabilities.Add(newavailability);
        //    return Ok();
        //    // return CreatedAtAction(nameof(GetServiceProviderAvailabilityAsync), new { id = newavailability.ServiceProvider }, newavailability);
        //}
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

