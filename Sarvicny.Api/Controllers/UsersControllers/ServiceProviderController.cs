using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;




namespace Sarvicny.Api.Controllers.UsersControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {

        private readonly IServiceProviderService _serviceProviderService;
        public ServiceProviderController(IServiceProviderService serviceProviderService)
        {
            _serviceProviderService = serviceProviderService;
        }


        [HttpPost]
        [Route("SetAvailability")]
        public async Task<IActionResult> AddAvailability(AvailabilityDto availabilityDto, string workerID)
        {

            var Response = await _serviceProviderService.AddAvailability(availabilityDto, workerID);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);

        }


        

        [HttpGet("GetServiceProviderAvailability/{providerId}")]
        public async Task<IActionResult> GetServiceProviderAvailabilityAsync(string providerId)

        {
            var Response = await _serviceProviderService.getAvailability(providerId);

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
            var Response = await _serviceProviderService.ApproveOrder(orderId);

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
            var Response = await _serviceProviderService.RejectOrder(orderId);

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
            var Response = await _serviceProviderService.CancelOrder(orderId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);
        }


        [HttpGet("ShowOrders")]
        public async Task<IActionResult> ShowOrderDetails(string orderId)
        {
            var Response = await _serviceProviderService.ShowOrderDetails(orderId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);
        }

        [HttpGet("GetProviders")]
        public async Task<IActionResult> GetAllServiceProviders()
        {
            var Response = await _serviceProviderService.GetAllServiceProviders();

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);
        }

        [HttpPost]
        [Route("RegisterService")]
        public async Task<IActionResult> RegisterService(string workerId, string serviceId, decimal price)
        {
            var response = await _serviceProviderService.RegisterServiceAsync(workerId, serviceId, price);

            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }


    }
}

