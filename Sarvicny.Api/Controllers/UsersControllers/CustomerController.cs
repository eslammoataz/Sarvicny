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

        public CustomerController(IAuthenticationService authenticationService, ICustomerService customerService, IOrderService orderService)
        {
            _authenticationService = authenticationService;
            _customerService = customerService;
            _orderService = orderService;
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
            var Response = await _customerService.RequestService(requestServiceDto, customerId);

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
            var Response = await _customerService.CancelRequestService(customerId, requestId);

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
        public async Task<IActionResult> OrderCart(string customerId, PaymentMethod paymentMethod)
        {
            var Response = await _customerService.OrderCart(customerId, paymentMethod);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);

        }

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
        [Route("getCustomerProfile")]
        public async Task<IActionResult> ShowCustomerProfile(string customerId)
        {
            var response = await _customerService.ShowCustomerProfile(customerId);

            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }
        [HttpGet]
        [Route("getCustomerOrdersLog")]
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
        [Route("CustomerRating")]
        public async Task<IActionResult> setRating(RatingDto rating)
        {
            var newRating = new OrderRating
            {
                OrderId = rating.OrderId,
                CustomerId = rating.CustomerId,
                ProviderId = rating.ServiceProviderId,
                CustomerRating = rating.customerRating,
                Comment = rating.Comment
            };
            var response = await _orderService.AddRatingCustomer(newRating);
            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


    }
}
