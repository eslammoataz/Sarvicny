using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProviderRepository _serviceProviderRepository;

        public ServiceProviderService(IUserRepository userRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepository)
        {
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _serviceProviderRepository = serviceProviderRepository;
        }

        public async Task<Response<string>> AddAvailability(AvailabilityDto availabilityDto, string workerId)
        {
            return new Response<string>()

            {
                Payload = null,
                Message = await _serviceProviderRepository.AddAvailability(availabilityDto, workerId)
            }; 
        }

        public async Task<Response<string>> ApproveOrder(string orderId)
        {
               return new Response<string>()

            {
                Payload = null,
                Message = await _serviceProviderRepository.ApproveOrder(orderId)
            };
}

        public async Task<Response<string>> CancelOrder(string orderId)
        {
            return new Response<string>()
            {
                Status = "Success",
                Message = await _serviceProviderRepository.CancelOrder(orderId),
                Payload = null

            };
        }

        public async Task<Response<ICollection<object>>> GetAllServiceProviders()
        {
            var serviceProviders = await _userRepository.GetAllServiceProviders();

            var serviceProvidersAsObjects = serviceProviders.Select(c => new
            {
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.isVerified
            }).ToList<object>();

            return new Response<ICollection<object>>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = serviceProvidersAsObjects
            };

    }

        public async Task<Response<ICollection<object>>> getAvailability(string workerId)
        {
            return new Response<ICollection<object>>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = await _serviceProviderRepository.getAvailability(workerId)
            };

        }

        public async Task<Response<ICollection<object>>> GetRegisteredServices(string workerId)
        {
            return new Response<ICollection<object>>()
            
                { Payload = await _serviceProviderRepository.GetRegisteredServices(workerId),
                 Message = "Success" 
                };
        }
        

        public async Task<Response<string>> RegisterService(string workerId, string serviceId, decimal price)
        {
            var user = await _serviceProviderRepository.FindByIdAsync(workerId);
            var spec = new BaseSpecifications<Service>();
            var service = _serviceRepository.GetServiceById(serviceId , spec);

            if (user == null || !(user is Worker))
            {
                return new Response<string> { isError = true, Message = "Worker Not Found" };
            }

            if (service == null)
            {
                return new Response<string> { isError = true, Message = "Service Not Found" };
            }

          

            if (await _serviceProviderRepository.WorkerExists(workerId) ||
                await _serviceProviderRepository.IsServiceRegisteredForWorker(workerId,serviceId))
            {
                return new Response<string> { isError = true, Message = "Worker already registered for this service" };
            }

            var workerService = new ProviderService()
            {
                ProviderID = workerId,
                ServiceID = serviceId,
                Price = price
            };

            await _serviceProviderRepository.AddProviderService(workerService);

            return new Response<string> { Message = "Worker registered for Service" };
        }

        public async Task<Response<string>> RejectOrder(string orderId)
        {
            return new Response<string>()
            {
                Status = "Success",
                Message = await _serviceProviderRepository.RejectOrder(orderId),
                Payload = null

            };
        }

        public  async Task<Response<object>> ShowOrderDetails(string orderId)
        {
            return new Response<object>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = await _serviceProviderRepository.ShowOrderDetails(orderId)
            };


        }
    }
}
