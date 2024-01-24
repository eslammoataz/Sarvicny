using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts.Authentication;



namespace Sarvicny.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var response = await _authenticationService.ConfirmEmailAsync(token, email);

            if (response.isError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    response);
            }
            
            return Ok(Response);
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            var response = await _authenticationService.Login(loginRequest);


            if (response.isError)
            {
                return Unauthorized(response);
            }

            // Add the token to the headers
            return Ok(response);
        }
    }
}