using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services;
using Sarvicny.Contracts.Authentication.Registers;
using Sarvicny.Contracts.Dtos;
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


        public CustomerController(IAuthenticationService authenticationService , ICustomerService customerService)
        {
            _authenticationService = authenticationService;
            _customerService = customerService;
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

            var Response = await _authenticationService.Register(user, role, registrationDto.Password);

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
        public async Task<IActionResult> RemoveFromCart(string customerId,string requestId)
        {
            var Response = await _customerService.CancelRequestService(customerId, requestId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }
            return Ok(Response);

        }

        [HttpPost]
        [Route("orderCart")]
        public async Task<IActionResult> OrderCart( string customerId)
        {
            var Response = await _customerService.OrderCart( customerId);

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
                return BadRequest("dsad");
            }
            return Ok(response);

        }


    }
}
