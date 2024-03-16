using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;


namespace Sarvicny.Api.Controllers.UsersControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        private readonly IServiceProviderService _serviceProviderService;
        private readonly IOrderService _orderService;

        public ServiceProviderController(IServiceProviderService serviceProviderService, IOrderService orderService)
        {
            _serviceProviderService = serviceProviderService;
            _orderService = orderService;
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


        [HttpGet("ShowOrderDetails")]
        public async Task<IActionResult> ShowOrderDetails(string orderId)
        {
            var Response = await _orderService.ShowOrderDetailsForProvider(orderId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }

            return Ok(Response);
        }

        //[HttpGet("GetProviders")]
        //public async Task<IActionResult> GetAllServiceProviders()
        //{
        //    var Response = await _serviceProviderService.GetAllServiceProviders();

        //    if (Response.isError)
        //    {
        //        return BadRequest(Response);
        //    }
        //    return Ok(Response);
        //}

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

        [HttpPost]
        [Route("AddDistrict")]
        public async Task<IActionResult> AddDistrict(string providerId, string districtName)
        {
            var response = await _serviceProviderService.AddDistrictToProvider(providerId, districtName);

            if (response.isError)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        //[HttpPost]
        //[Route("RequestDistrict")]
        //public async Task<IActionResult> RequestNewDistrictToBeAdded(string districtName)
        //{
        //    var response = await _serviceProviderService.RequestNewDistrictToBeAdded(districtName);

        //    if (response.isError)
        //    {
        //        return BadRequest(response);
        //    }

        //    return Ok(response);
        //}

        //[HttpGet("getProviderDistricts/{providerid}")]
        //public async Task<IActionResult> GetProviderDistricts(string providerID)
        //{
        //    var response = await _serviceProviderService.GetProviderDistricts(providerID);

        //    if (response.isError)
        //    {
        //        return NotFound(response);
        //    }


        //    return Ok(response);
        //}

        [HttpGet("getAllOrders")]
        public async Task<IActionResult> getAllOrders(string providerID)
        {
            var response = await _serviceProviderService.getAllOrders(providerID);

            if (response.isError)
            {
                return NotFound(response);
            }


            return Ok(response);
        }

        [HttpGet("getAllApprovedOrders")]
        public async Task<IActionResult> getAllApprovedOrders(string providerID)
        {
            var response = await _serviceProviderService.getAllApprovedOrders(providerID);

            if (response.isError)
            {
                return NotFound(response);
            }


            return Ok(response);
        }

        [HttpGet("getAllRequestededOrders")]
        public async Task<IActionResult> getAllRequestededOrders(string providerID)
        {
            var response = await _serviceProviderService.getAllRequestedOrders(providerID);

            if (response.isError)
            {
                return NotFound(response);
            }


            return Ok(response);
        }

        [HttpGet("getRegisteredServices")]
        public async Task<IActionResult> getRegisteredServices(string providerID)
        {
            var response = await _serviceProviderService.getRegisteredServices(providerID);

            if (response.isError)
            {
                return NotFound(response);
            }
            return Ok(response);
        }


        [HttpGet]
        [Route("ShowProviderProfile")]
        public async Task<IActionResult> ShowProviderProfile(string providerId)
        {
            var response = await _serviceProviderService.ShowProviderProfile(providerId);

            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }

        [HttpPost]
        [Route("ServiceProivderRating")]
        public async Task<IActionResult> setRating(RatingDto rating)
        {
            var newRating = new OrderRating
            {
                OrderId = rating.OrderId,
                CustomerId = rating.CustomerId,
                ProviderId = rating.ServiceProviderId,
                ServiceProviderRating = rating.providerRating,
                Comment = rating.Comment
            };
            var response = await _orderService.AddRatingServiceProvider(newRating);
            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);


        }

    }
}