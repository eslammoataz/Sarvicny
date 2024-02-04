using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IWorkerRepository _workerRepository;
        public WorkerService(IWorkerRepository workerRepository) {  _workerRepository = workerRepository; }

        public async Task<Response<object>> ShowWorkerProfile(string workerId)
        {
            var spec = new BaseSpecifications<Worker>(p => p.Id == workerId);
            var worker = await _workerRepository.FindByIdAsync(spec);
            if (worker == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Provider Not Found",
                    Payload = null,
                    isError = true
                };
            }
            var profile = new
            {
                worker.FirstName,
                worker.LastName,
                worker.UserName,
                worker.Email,
                worker.PhoneNumber,
                worker.NationalID,
                worker.CriminalRecord

            };
            return new Response<object>()
            {
                Payload = profile,
                isError = false,
                Message = "Success"

            };
        }

    }
}
