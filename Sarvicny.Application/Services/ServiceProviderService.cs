using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
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
        private readonly IOrderRepository _orderRepository;


        public ServiceProviderService(IUserRepository userRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepository,IOrderRepository orderRepository)
        {
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _serviceProviderRepository = serviceProviderRepository;
            _orderRepository = orderRepository;
        }

        public async Task<Response<object>> AddAvailability(AvailabilityDto availabilityDto, string workerId)
        {
            var provider = _serviceProviderRepository.FindByIdAsync(workerId);
            if (provider == null)
            {
                return new Response<object>()

                {
                    Payload = null,
                    Message = "Provider Not Found"
                };
            }
            return new Response<object>()

            {
                Payload = await _serviceProviderRepository.AddAvailability(availabilityDto, workerId),
                Message = "success"
            }; 
        }

        public async Task<Response<object>> ApproveOrder(string orderId)
        {
            var spec = new OrderWithCustomerSpecification();
            var order = _orderRepository.GetOrderById(orderId, spec);
            if (order == null)
            {
                return new Response<object>()

                {
                    Payload = null,
                    Message = "Order Not Found"
                };

            }
            return new Response<object>()

            {
                Payload = await _orderRepository.ApproveOrder(orderId,spec),
                Message = "Success"
            };
}

        public async Task<Response<object>> CancelOrder(string orderId)
        {
            var spec = new OrderWithCustomerSpecification();
            var order = _orderRepository.GetOrderById(orderId, spec);
            if (order == null)
            {
                return new Response<object>()

                {
                    Payload = null,
                    Message = "Order Not Found"
                };

            }
       
            var Response= new Response<object>()
            {
                Message = "success",
                Payload = await _orderRepository.CancelOrder(orderId, spec),
              
            };
            if (Response.Payload == null)
            {
                return new Response<object>()
                {
                    Message = "Order was not originally approved to be Canceled",
                    Payload = null
                };
            }
            return Response;
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

        public async Task<Response<ICollection<object>>> getAvailability(string providerId)
        {
            var spec = new ProviderWithAvailabilitesSpecification();
            var provider= _serviceProviderRepository.FindByIdWithSpecificationAsync(providerId, spec);

            if (provider == null)
            {
                return new Response<ICollection<object>>()
                {
                    Message = "Provider Not found",
                    Payload= null
                };
            }

            return new Response<ICollection<object>>()
            {
                
                Message = "success",
                Payload = await _serviceProviderRepository.getAvailability(providerId,spec)
            };

        }

        public async Task<Response<ICollection<object>>> GetRegisteredServices(string workerId)
        {
            var spec = new ServiceProviderWithServiceSpecificationcs();
            var provider = _serviceProviderRepository.FindByIdWithSpecificationAsync(workerId,spec);
           
            if (provider == null)
            {
                return new Response<ICollection<object>>()

                {
                    Payload = null,
                    Message = "Provider is not found"
                };
            }

            return new Response<ICollection<object>>()

            {
                Payload = await _serviceProviderRepository.GetRegisteredServices(workerId, spec),
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

        public async Task<Response<object>> RejectOrder(string orderId)
        {
            var spec = new OrderWithCustomerSpecification();
            var order = _orderRepository.GetOrderById(orderId, spec);
            if (order == null)
            {
                return new Response<object>()

                {
                    Payload = null,
                    Message = "Order Not Found"
                };

            }
            return new Response<object>()

            {
                Payload = await _orderRepository.RejectOrder(orderId, spec),
                Message = "Success"
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
