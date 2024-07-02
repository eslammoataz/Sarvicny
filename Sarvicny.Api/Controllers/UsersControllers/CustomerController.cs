using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts.Authentication.Registers;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;

namespace Sarvicny.Api.Controllers.UsersControllers
{

    //[Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;


        public CustomerController(IAuthenticationService authenticationService, ICustomerService customerService, IOrderService orderService, IPaymentService paymentService)
        {
            _authenticationService = authenticationService;
            _customerService = customerService;
            _orderService = orderService;
            _paymentService = paymentService;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterCustomer registrationDto, string role)
        {
            var user = new Customer()
            {
                Email = registrationDto.Email,
                UserName = registrationDto.UserName,
                PhoneNumber = registrationDto.PhoneNumber,
                Address = registrationDto.Address,
                LastName = registrationDto.LastName,
                FirstName = registrationDto.FirstName,
                DistrictName = registrationDto.DistrictName,
            };

            var Response = await _authenticationService.Register(user, role, registrationDto.UserType, registrationDto.Password);

            if (Response.isError)
                return BadRequest(Response);

            return Ok(Response);

        }


        [HttpPost]
        [Route("addtocart")]
        public async Task<IActionResult> AddToCart(RequestServiceDto requestServiceDto, string customerId)
        {
            var Response = await _customerService.addToCart(requestServiceDto, customerId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);

        }

        [HttpPost]
        [Route("removeFromCart")]
        public async Task<IActionResult> RemoveFromCart(string customerId, string requestId)
        {
            var Response = await _customerService.RemoveFromCart(customerId, requestId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);

        }

        /// <summary>
        /// Order items in the customer's cart.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="paymentMethod">The payment method. Valid values: Paypal, Paymob.</param>
        /// <returns>The response containing the result of the operation.</returns>

        [HttpPost]
        [Route("orderCart")]
        public async Task<IActionResult> GetTransactionOrder(string customerId, PaymentMethod paymentMethod)
        {
            var Response = await _customerService.OrderCart(customerId, paymentMethod);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);

        }

        [HttpPost]
        [Route("payTransaction")]
        public async Task<IActionResult> PayTransaction(string transactionID)
        {
            var Response = await _paymentService.PayOrder(transactionID);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);

        }

        [HttpPost]
        [Route("refundOrder")]
        public async Task<IActionResult> Refund(string transactionPaymentId, string orderId)
        {
            var response = await _paymentService.RefundOrder(transactionPaymentId, orderId);

            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }


        [HttpPost]
        [Route("AddProviderToFav")]
        public async Task<IActionResult> AddProviderToFav(string providerId, string customerId)
        {
            var Response = await _customerService.AddProviderToFav(providerId, customerId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);

        }

        [HttpPost]
        [Route("RemoveFavProvider")]
        public async Task<IActionResult> RemoveFavProvider(string customerId, string providerId)
        {
            var Response = await _customerService.RemoveFavProvider(customerId, providerId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);

        }

        [HttpGet]
        [Route("getCustomerFavourites/{customerId}")]
        public async Task<IActionResult> getCustomerFavourites(string customerId)
        {

            var response = await _customerService.getCustomerFavourites(customerId);
            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }



        //[HttpPost]
        //[Route("payOrder/{orderId}")]
        //public async Task<IActionResult> PayOrder(string orderId, PaymentMethod paymentMethod)
        //{
        //    var Response = await _customerService.PayOrder(orderId, paymentMethod);

        //    if (Response.isError)
        //    {
        //        return BadRequest(Response);
        //    }
        //    return Ok(Response);

        //}

        [HttpGet]
        [Route("getCart")]
        public async Task<IActionResult> GetCart(string customerId)
        {
            var response = await _customerService.GetCustomerCart(customerId);

            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }


        [HttpGet]
        [Route("getOrderStatus")]
        public async Task<IActionResult> GetOrderStatus(string orderId)
        {
            var response = await _orderService.ShowOrderStatus(orderId);

            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }
        [HttpGet]
        [Route("getCustomerProfile/{customerId}")]
        public async Task<IActionResult> ShowCustomerProfile(string customerId)
        {
            var response = await _customerService.ShowCustomerProfile(customerId);

            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }
        [HttpPost]
        [Route("updateCustomerProfile/{customerId}")]
        public async Task<IActionResult> UpdateCustomerProfile(UpdateCustomerDto updateCustomerDto, string customerId)
        {
            var response = await _customerService.UpdateCustomerProfile(updateCustomerDto, customerId);

            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }
        [HttpGet]
        [Route("getCustomerOrdersLog/{customerId}")]
        public async Task<IActionResult> getCustomerOrdersLog(string customerId)
        {
            var response = await _customerService.ViewLogRequest(customerId);

            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }



        [HttpPost]
        [Route("addCustomerRating/{orderId}")]
        public async Task<IActionResult> AddCustomerRating(RatingDto customerRatingDto, string orderId)
        {

            var response = await _orderService.AddCustomerRating(customerRatingDto, orderId);
            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("GetCustomerRating/{orderId}")]
        public async Task<IActionResult> GetCustomerRating(string orderId)
        {

            var response = await _orderService.GetCustomerRatingForOrder(orderId);
            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpPost]
        [Route("getAllMatchedProviderSortedbyFav")]
        public async Task<IActionResult> GetAllMatchedProviderSortedbyFav([FromBody] MatchingProviderDto matchingProviderDto)
        {

            var response = await _orderService.GetAllMatchedProviderSortedbyFav(matchingProviderDto);
            if (response.isError)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("getFirstSuggestionsSortedbyFav")]
        public async Task<IActionResult> getFirstSuggestionsSortedbyFav([FromBody] MatchingProviderDto matchingProviderDto)
        {

            var response = await _orderService.SuggestNewProvidersIfNoMatchesFoundLevel1(matchingProviderDto);
            if (response.isError)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("getSecondSuggestionsSortedbyFav")]
        public async Task<IActionResult> getSecondSuggestionsSortedbyFav([FromBody] MatchingProviderDto matchingProviderDto)
        {

            var response = await _orderService.SuggestNewProvidersIfNoMatchesFoundLevel2(matchingProviderDto);
            if (response.isError)
            {
                return NotFound(response);
            }
            return Ok(response);
        }


        [HttpPost]
        [Route("getAllReAssignedCartRequests/{customerId}")]
        public async Task<IActionResult> getAllReAssignedCartRequests(string customerId)
        {
            var response = await _customerService.GetReAssignedCartServiceRequests(customerId);
            if (response.isError)
            {
                return NotFound(response);
            }
            return Ok(response);
        }


        [HttpGet("customerOrders/{customerId}/status/canceledByOrder")]
        public async Task<IActionResult> GetCustomerCanceledOrders(string customerId)
        {
            var response = await _customerService.GetCustomerCanceledOrders(customerId);

            if (response.isError)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }

}
