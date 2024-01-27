using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
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

        public async Task<Response<ICollection<object>>> GetRegisteredServices(string workerId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<object>> AddAvailability(AvailabilityDto availabilityDto, string workerId)
        {
            var spec = new BaseSpecifications<Provider>();
            var provider = _serviceProviderRepository.FindByIdAsync(spec);
            if (provider == null)
            {
                return new Response<object>()

                {
                    isError = true,
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

        public async Task<Response<ICollection<object>>> getAvailability(string workerId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<object>> AddAvailabilitySlots(TimeSlotDto slotDto, string availabilityId)
        {
           var avaliability = await _serviceProviderRepository.AddAvailabilitySlots(slotDto, availabilityId);
            if(avaliability == null) 
            {
                return new Response<object>()

                {   isError= true,
                    Payload = null,
                    Message = "Provider Not Found"
                };
            }
            else
            {
                _unitOfWork.Commit();
                return new Response<object>()

                {
                    Payload = avaliability,
                    Message = "success"
                };
                
            }  
        }

        public async Task<Response<object>> ApproveOrder(string orderId)
        {
            var spec = new OrderWithCustomerSpecification();
            var order = _orderRepository.GetOrderById( spec);
            if (order == null)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Order Not Found",
                   
                };

            }
            return new Response<object>()

            {
                Payload = await _orderRepository.ApproveOrder(spec),
                Message = "Success"
            };
}

        public async Task<Response<object>> CancelOrder(string orderId)
        {
            var spec = new OrderWithCustomerSpecification();
            var order = _orderRepository.GetOrderById(spec);
            if (order == null)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Order Not Found"
                };

            }
       
            var Response= new Response<object>()
            {
                Message = "success",
                Payload = await _orderRepository.CancelOrder(spec),
              
            };
            if (Response.Payload == null)
            {
                return new Response<object>()
                {
                    isError = true,
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

        // public async Task<Response<ICollection<object>>> getAvailability(string providerId)
        // {
        //     var spec = new ProviderWithAvailabilitesSpecification();
        //     var provider= _serviceProviderRepository.FindByIdWithSpecificationAsync(providerId, spec);
        //
        //     if (provider == null)
        //     {
        //         return new Response<ICollection<object>>()
        //         {
        //             isError = true,
        //             Message = "Provider Not found",
        //             Payload= null
        //         };
        //     }
        //
        //     return new Response<ICollection<object>>()
        //     {
        //         
        //         Message = "success",
        //         Payload = await _serviceProviderRepository.getAvailability(providerId,spec)
        //     };
        //
        // }

        // public async Task<Response<ICollection<object>>> GetRegisteredServices(string workerId)
        // {
        //     var spec = new ServiceProviderWithServiceSpecificationcs();
        //     var provider = _serviceProviderRepository.FindByIdWithSpecificationAsync(workerId,spec);
        //    
        //     if (provider == null)
        //     {
        //         return new Response<ICollection<object>>()
        //
        //         {
        //
        //             Payload = null,
        //             Message = "Provider is not found"
        //         };
        //     }
        //
        //     return new Response<ICollection<object>>()
        //
        //     {
        //         Payload = await _serviceProviderRepository.GetRegisteredServices(workerId, spec),
        //         Message = "Success"
        //     };
        // }


        public async Task<Response<ProviderService>> RegisterServiceAsync(string workerId, string serviceId, decimal price)
        {
            var spec1 = new ProviderWithServicesSpecification();
            
            var worker = await _serviceProviderRepository.FindByIdAsync(spec1);
            var spec = new ServiceWithProvidersSpecification(serviceId);
            var service = await _serviceRepository.GetServiceById(spec);

            var response = new Response<ProviderService>();
            
            if (worker == null)
            {
                response.isError = true;
                response.Status = "failed";
                response.Message = "Worker Not Found";
                response.Errors.Add("Worker Not Found");
                return response;
            }
            if(worker.isVerified=false)
            {
                response.isError = true;
                response.Status = "failed";
                response.Message = "Worker Not Verified";
                response.Errors.Add("Worker Not Verified");
                return response;
            }

            if (service == null)
            {
                response.isError = true;
                response.Status = "failed";
                response.Message = "Service Not Found";
                response.Errors.Add("Service Not Found");
                return response;
            }
            
            var isServiceRegistered = worker.ProviderServices.Any(ws => ws.ServiceID == serviceId);
            if (isServiceRegistered)
            {
                response.isError = true;
                response.Status = "failed";
                response.Message = "Service Already Registered";
                response.Errors.Add("Service Already Registered");
                return response;
            }

            var workerService = new ProviderService()
            {
                ProviderID = workerId,
                ServiceID = serviceId,
                Price = price
            };

            await _serviceProviderRepository.AddProviderService(workerService);
            worker.ProviderServices.Add(workerService);
            service.ProviderServices.Add(workerService);
            

            response.Status = "Success";
            response.Message = "Action Done Successfully";
            response.Payload = workerService;
            return response;
            
        }

        public async Task<Response<object>> RejectOrder(string orderId)
        {
            var spec = new OrderWithCustomerSpecification();
            var order = _orderRepository.GetOrderById(spec);
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
                Payload = await _orderRepository.RejectOrder(spec),
                Message = "Success"
            };
         }

        public  async Task<Response<object>> ShowOrderDetails(string orderId)
        {
            var spec = new OrderWithCustomers_Carts();
            var order = _orderRepository.ShowOrderDetails(spec);
            if (order == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Order Not Found",
                    Payload = null
                };
            }

            else
            {
                return new Response<object>()
                {
                    Status = "Success",
                    Message = "Action Done Successfully",
                    Payload = order
                };
            }


        }
    }
}
