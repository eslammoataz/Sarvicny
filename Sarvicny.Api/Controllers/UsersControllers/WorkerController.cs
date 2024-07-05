using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Domain.Entities.Users.ServicProviders;

namespace Sarvicny.Api.Controllers.UsersControllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   [Authorize(Roles = "ServiceProvider ,Admin")]
    public class WorkerController : ControllerBase
    {

        private readonly IAuthenticationService authenticationService;
        private readonly IServiceProviderService _serviceProviderService;
        // private readonly IWorkerService workerService;


        public WorkerController(IAuthenticationService authenticationService, IServiceProviderService serviceProviderService)
        {

            this.authenticationService = authenticationService;
            this._serviceProviderService = serviceProviderService;
            // this.workerService = workerService;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterWorkerDto registrationDto, string role)
        {

            var user = new Worker()
            {
                Email = registrationDto.Email,
                UserName = registrationDto.UserName,
                PhoneNumber = registrationDto.PhoneNumber,
                LastName = registrationDto.LastName,
                FirstName = registrationDto.FirstName,
                NationalID = registrationDto.NationalID,

                IsVerified = false,
                //photos
            };

            var Response = await authenticationService.Register(user, role, registrationDto.UserType, registrationDto.Password);

            if (Response.isError)
                return BadRequest(Response);

            return Ok(Response);

        }

        [HttpPost]
        [Route("uploadFile")]
        public async Task<IActionResult> UploadFile([FromBody] ImageUploadDto imageUploadDto, ProviderFileTypes fileName, string providerId)
        {

            var Response = await _serviceProviderService.UploadFileForWoker(imageUploadDto, fileName, providerId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }

            return Ok(Response);

        }
        [HttpGet]
        [Route("getWorkerImage")]
        public async Task<IActionResult> getWorkerImage(string providerId)
        {

            var Response = await _serviceProviderService.GetImageForWorker(providerId);

            if (Response.isError)
            {
                return BadRequest(Response);
            }

            return Ok(Response);

        }
    }
}
