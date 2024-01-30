using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.NewFolder;
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


        public ServiceProviderService(IUserRepository userRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepository, IOrderRepository orderRepository)
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
           
            availabilityDto.AvailabilityDate = availabilityDto.AvailabilityDate.Value.Date;

            var spec = new ProviderWithAvailabilitesSpecification(workerId);
            var provider =  await _serviceProviderRepository.FindByIdAsync(spec);
            if (provider == null)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider Not Found"
                };
            }

            var availiabilities = await _serviceProviderRepository.AddAvailability(availabilityDto, spec);
            provider.Availabilities.Add(availiabilities);
            _unitOfWork.Commit();
           var slots=  availiabilities.Slots.Select(s => new
            {
                s.StartTime,
                s.EndTime
            }).ToList();
            object result = new 
            {
                availiabilities.AvailabilityDate,
                availiabilities.DayOfWeek,
                slots,
                availiabilities.ServiceProviderID
            };
            return new Response<object>()

            {
                isError = false,
                Payload = result,
                Message = "success"
            };
        }

        public async Task<Response<List<object>>> getAvailability(string workerId)
        {
            var spec = new ProviderWithAvailabilitesSpecification(workerId);
            var provider = await _serviceProviderRepository.FindByIdAsync(spec);
            if (provider == null)
            {
                return new Response<List<object>>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider Not Found"
                };
            }
            var availability = await _serviceProviderRepository.getAvailability(spec);

             var spec2 = new AvailaibiltyWithSlotsSpecification(workerId);
            var slots = await _serviceProviderRepository.getAvailabilitySlots(spec2);
            foreach(var val in availability)
            {
                val.Slots = slots;
            }

            var avail = availability.Select(a => new
            {
                a.ProviderAvailabilityID,
                a.AvailabilityDate,
                a.DayOfWeek,
                slots = a.Slots.Select(s => new { s.StartTime, s.EndTime }).ToList<object>(),

            }).ToList<object>();
            
            return new Response<List<object>>()

            {
                isError = false,
                Payload = avail,
                Message = "Success"
            };
        }

        
        public async Task<Response<object>> ApproveOrder(string orderId)
        {
            
            var spec = new OrderWithCustomerSpecification();
            var order = await _orderRepository.GetOrder(spec);
            
            if (order == null)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Order Not Found",
                    Errors = new List<string>() { "Error with order" },

                };

            }

            var result = await _orderRepository.ApproveOrder(spec);
            var output = new
            {
                result.OrderID,
                result.OrderStatusID,
                result.OrderStatus.StatusName,
                result.CustomerID,
            };
            
            _unitOfWork.Commit();
            return new Response<object>()
            {
                Payload =output ,
                Message = "Success",
                isError = false,
            };
        }

        public async Task<Response<object>> CancelOrder(string orderId)
        {
            var spec = new OrderWithCustomerSpecification();
            var order = _orderRepository.GetOrder(spec);
            if (order == null)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Order Not Found"
                };

            }

            var Response = new Response<object>()
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


        public async Task<Response<object>> RegisterServiceAsync(string workerId, string serviceId, decimal price)
        {
            var spec1 = new ProviderWithServicesAndAvailabilitiesSpecification(workerId);

            var worker = await _serviceProviderRepository.FindByIdAsync(spec1);
            var spec = new ServiceWithProvidersSpecification(serviceId);
            var service = await _serviceRepository.GetServiceById(spec);

            var response = new Response<object>();

            if (worker == null)
            {
                response.isError = true;
                response.Status = "failed";
                response.Message = "Worker Not Found";
                response.Errors.Add("Worker Not Found");
                return response;
            }
            if (worker.isVerified = false)
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
                Provider=worker,
                Service=service,
                ServiceID = serviceId,
                Price = price
            };


            await _serviceProviderRepository.AddProviderService(workerService);
            worker.ProviderServices.Add(workerService);
            service.ProviderServices.Add(workerService);
            var result = new
            {
                workerService.ProviderID,
                workerService.ServiceID,
                workerService.Price
            };
            _unitOfWork.Commit();


            response.Status = "Success";
            response.Message = "Action Done Successfully";
            response.Payload = result;
            return response;

        }

        public async Task<Response<object>> RejectOrder(string orderId)
        {
            var spec = new OrderWithCustomerSpecification();
            var order = _orderRepository.GetOrder(spec);
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

        public async Task<Response<object>> ShowOrderDetails(string orderId)
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

        public async Task<Response<List<object>>> getAllOrders(String workerId)
        {
            var spec = new OrderWithCustomers_Carts();
            var orders =await _orderRepository.GetAllOrders(spec);
            var provider = await _userRepository.GetUserByIdAsync(workerId);
            if (provider == null)
            {
                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "Provider Not Found",
                    Payload = null,
                    isError = true
                };
            };

            List<object> result = new List<object>();
            foreach (var order in orders)
            {
                if(order.Customer.Cart.ServiceRequests.Any(s => s.providerService.ProviderID == workerId))
                {
                    var ordersAsobject = new
                    {
                        orderId = order.OrderID,
                        customerId = order.Customer.Id,
                        customerFN = order.Customer.FirstName,
                        orderStatus = order.OrderStatus.StatusName,
                        orderPrice = order.TotalPrice,


                    };
                    result.Add(ordersAsobject);
                }
                else
                {
                    continue;
                }
               
            }
            
            if(result.Count == 0)
            {
                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "No Orders Found",
                    Payload = null,
                    isError = true
                };
            }
           
            return new Response<List<object>>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = result,

            };

        }

        public async Task<Response<List<object>>> getAllApprovedOrders(string workerId)
        {
            var spec = new OrderWithCustomers_Carts();
            var orders = await _orderRepository.GetAllOrders(spec);
            var provider = await _userRepository.GetUserByIdAsync(workerId);
            if (provider == null)
            {
                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "Provider Not Found",
                    Payload = null,
                    isError = true
                };
            };

            List<object> result = new List<object>();
            foreach (var order in orders)
            {
                if (order.OrderStatusID == "2") //2 means approved (3awzem n seed elklam da)
                {
                    if (order.Customer.Cart.ServiceRequests.Any(s => s.providerService.ProviderID == workerId))
                    {
                        var ordersAsobject = new
                        {
                            orderId = order.OrderID,
                            customerId = order.Customer.Id,
                            customerFN = order.Customer.FirstName,
                            orderStatus = order.OrderStatus.StatusName,
                            orderPrice = order.TotalPrice,


                        };
                        result.Add(ordersAsobject);
                    }

                    else
                    {
                        continue;
                    }
                }
                else { continue; }

            }

            if (result.Count == 0)
            {
                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "No Orders Found",
                    Payload = null,
                    isError = true
                };
            }

            return new Response<List<object>>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = result,

            };

        }

        public async Task<Response<List<object>>> getAllRequestedOrders(string workerId)
        {
            var spec = new OrderWithCustomers_Carts();
            var orders = await _orderRepository.GetAllOrders(spec);
            var provider = await _userRepository.GetUserByIdAsync(workerId);
            if (provider == null)
            {
                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "Provider Not Found",
                    Payload = null,
                    isError = true
                };
            };

            List<object> result = new List<object>();
            foreach (var order in orders)
            {
                if (order.OrderStatusID == "1") // 1 means request
                {
                    if (order.Customer.Cart.ServiceRequests.Any(s => s.providerService.ProviderID == workerId))
                    {
                        var ordersAsobject = new
                        {
                            orderId = order.OrderID,
                            customerId = order.Customer.Id,
                            customerFN = order.Customer.FirstName,
                            orderStatus = order.OrderStatus.StatusName,
                            orderPrice = order.TotalPrice,


                        };
                        result.Add(ordersAsobject);
                    }

                    else
                    {
                        continue;
                    }
                }
                else { continue; }

            }

            if (result.Count == 0)
            {
                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "No Orders Found",
                    Payload = null,
                    isError = true
                };
            }

            return new Response<List<object>>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = result,

            };

        }
    }
}
