using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
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
        [Route("setAvailability")]
        public async Task<IActionResult> AddAvailability(AvailabilityDto availabilityDto, string workerID)
        {
            var Response = await _serviceProviderService.AddAvailability(availabilityDto, workerID);

            if (Response.isError)
            {
                return BadRequest(Response);
            }

            return Ok(Response);
        }

        [HttpPost]
        [Route("removeAvailability")]
        public async Task<IActionResult> RemoveAvailability(string availabilityId, string providerId)
        {
            var Response = await _serviceProviderService.RemoveAvailability(availabilityId, providerId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }

            return Ok(Response);
        }


        [HttpGet("getServiceProviderAvailability/{providerId}")]
        public async Task<IActionResult> GetServiceProviderAvailabilityAsync(string providerId)

        {
            var Response = await _serviceProviderService.getAvailability(providerId);

            if (Response == null)
            {
                return Ok("There is no availability for this worker");
            }

            return Ok(Response);
        }

        //[HttpPost]
        //[Route("approveOrder")]
        //public async Task<IActionResult> ApproveOrder(string orderId)
        //{
        //    var Response = await _serviceProviderService.ApproveOrder(orderId);

        //    if (Response.isError)
        //    {
        //        return BadRequest(Response);
        //    }

        //    return Ok(Response);
        //}

        //[HttpPost]
        //[Route("rejectOrder")]
        //public async Task<IActionResult> RejectOrder(string orderId)
        //{
        //    var Response = await _serviceProviderService.RejectOrder(orderId);

        //    if (Response.isError)
        //    {
        //        return BadRequest(Response);
        //    }

        //    return Ok(Response);
        //}

        [HttpPost]
        [Route("cancelOrder")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            var Response = await _serviceProviderService.CancelOrder(orderId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }

            return Ok(Response);
        }

        [HttpPost]
        [Route("setOrderStatus")]
        public async Task<IActionResult> SetOrderStatus(string orderId,OrderStatusEnum status)
        {
            var Response = await _serviceProviderService.SetOrderStatus(orderId,status);

            if (Response.isError)
            {
                return BadRequest(Response);
            }

            return Ok(Response);
        }


        [HttpGet("showOrderDetails")]
        public async Task<IActionResult> ShowOrderDetails(string orderRequestId)
        {
            var Response = await _orderService.ShowAllOrderDetailsForProvider(orderRequestId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }

            return Ok(Response);
        }



        [HttpPost]
        [Route("registerService")]
        public async Task<IActionResult> RegisterService(string workerId, string serviceId, decimal price)
        {
            var response = await _serviceProviderService.RegisterServiceAsync(workerId, serviceId, price);

            if (response.isError)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }



        [HttpGet("getAllOrdersForProvider")]
        public async Task<IActionResult> getAllOrders(string providerID)
        {
            var response = await _serviceProviderService.getAllOrdersForProvider(providerID);

            if (response.isError)
            {
                return BadRequest(response);
            }


            return Ok(response);
        }

        //[HttpGet("getAllApprovedOrders")]
        //public async Task<IActionResult> getAllApprovedOrders(string providerID)
        //{
        //    var response = await _serviceProviderService.getAllApprovedOrderForProvider(providerID);

        //    if (response.isError)
        //    {
        //        return NotFound(response);
        //    }


        //    return Ok(response);
        //}

        [HttpGet("getAllRequestededOrders")]
        public async Task<IActionResult> getAllRequestededOrders(string providerID)
        {
            var response = await _serviceProviderService.getAllPendingOrPaidOrderForProvider(providerID);

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
        [Route("showProviderProfile")]
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
        [Route("addProviderRating/{orderId}")]
        public async Task<IActionResult> AddPoviderRating(RatingDto providerRatingDto, string orderId)
        {

            var response = await _orderService.AddProviderRating(providerRatingDto, orderId);
            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("GetProviderRating/{orderId}")]
        public async Task<IActionResult> GetProviderRating(string orderId)
        {

            var response = await _orderService.GetProviderRatingForOrder(orderId);
            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        //[HttpGet]
        //[Route("getProviderServicePrice")]
        //public async Task<IActionResult> getProviderServicePrice(string providerId,string serviceId)
        //{

        //    var response = await _serviceProviderService.getProviderServicePrice( providerId,  serviceId);
        //    if (response.isError)
        //    {
        //        return BadRequest(response);
        //    }
        //    return Ok(response);
        //}

        [HttpGet]
        [Route("GetProviderWallet/{providerId}")]
        public async Task<IActionResult> GetProviderWallet(string providerId)
        {

            var response = await _serviceProviderService.getWallet(providerId);
            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}