using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services;
using Sarvicny.Contracts.Authentication.Registers;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Contracts.Dtos;

namespace Sarvicny.Api.Controllers.UsersControllers
{

   
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {

        private readonly IAuthenticationService authenticationService;
        private readonly IWorkerService workerService;


        public WorkerController(IAuthenticationService authenticationService, IWorkerService workerService)
        {

            this.authenticationService = authenticationService;
            this.workerService = workerService;
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
                CriminalRecord = registrationDto.CriminalRecord,
                NationalID = registrationDto.NationalID,
                IsVerified = false
                 
                //photos
            };

            var Response = await authenticationService.Register(user, role, registrationDto.Password);

            if (Response.isError)
                return BadRequest(Response);

            return Ok(Response);

        }

        [HttpGet]
        [Route("getWorkerProfile")]
        public async Task<IActionResult> ShowWorkerProfile(string workerId)
        {
            var response = await workerService.ShowWorkerProfile(workerId);

            if (response.isError)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }


    }
}
