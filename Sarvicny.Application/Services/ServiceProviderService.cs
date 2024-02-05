using Microsoft.Extensions.DependencyInjection;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Specifications.NewFolder;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using System;
using System.ComponentModel;
using System.Dynamic;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sarvicny.Application.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProviderRepository _serviceProviderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;

        private IOrderService _orderService;


        public ServiceProviderService(IUserRepository userRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepository, IOrderRepository orderRepository, ICustomerRepository customerRepository, IOrderService orderService, IEmailService emailService)
        {
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _serviceProviderRepository = serviceProviderRepository;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _orderService = orderService;
            _emailService = emailService;
        }
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
            //if (worker.isVerified == false)   // b2ena n approve abl ma y verify
            //{
            //    response.isError = true;
            //    response.Status = "failed";
            //    response.Message = "Worker Not Verified";
            //    response.Errors.Add("Worker Not Verified");
            //    return response;
            //}

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
                Provider = worker,
                Service = service,
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
        public async Task<Response<ICollection<object>>> GetRegisteredServices(string workerId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<object>> AddAvailability(AvailabilityDto availabilityDto, string workerId)
        {

            availabilityDto.AvailabilityDate = availabilityDto.AvailabilityDate.Value.Date;

            var spec = new ProviderWithAvailabilitesSpecification(workerId);
            var provider = await _serviceProviderRepository.FindByIdAsync(spec);
            if (provider == null)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider Not Found"
                };
            }
            if (provider.IsVerified == false)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider Not Verified"
                };
            }


            var availiabilities = await _serviceProviderRepository.AddAvailability(availabilityDto, spec);
            provider.Availabilities.Add(availiabilities);
            _unitOfWork.Commit();
            var slots = availiabilities.Slots.Select(s => new
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
            foreach (var val in availability)
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

            var spec = new OrderWithRequestsSpecification(orderId);
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
            if (order.OrderStatusID == "2")
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Order is already Approved",
                    Errors = new List<string>() { "Error with order" },

                };
            }


            await _orderRepository.ApproveOrder(order);
            _unitOfWork.Commit();
            var details = await _orderService.ShowOrderDetailsForProvider(orderId);


            var customer = order.Customer;
            var message = new EmailDto(customer.Email!, "Sarvicny: Order Approved", "Thank you for using our system! Your order is approved ");  // akedd momkn yet7sn

            _emailService.SendEmail(message);

            return new Response<object>()

            {
                isError = false,
                Payload = details,
                Message = "Order Approved Succesfully",


            };
        }

        public async Task<Response<object>> CancelOrder(string orderId)
        {
            var spec = new OrderWithRequestsSpecification(orderId);
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
            if (order.OrderStatusID != "2") // if not approved
            {

                if (order.OrderStatusID == "4") // if canceled
                {

                    return new Response<object>()
                    {
                        isError = true,
                        Payload = null,
                        Message = "Order is already canceled",
                        Errors = new List<string>() { "Error with order" },

                    };
                }
                return new Response<object>()
                {
                    isError = true,
                    Payload = null,
                    Message = "Order is not intially approved",
                    Errors = new List<string>() { "Error with order" },

                };

            }
            await _orderRepository.CancelOrder(order);
            _unitOfWork.Commit();

            var customer = order.Customer;
            var message = new EmailDto(customer.Email!, "Sarvicny: Canceled", "Unfortunally! Your order is Canceled ");  // akedd momkn yet7sn

            _emailService.SendEmail(message);

            ///ReAsignnnnnn??
            var details = await _orderService.ShowOrderDetailsForProvider(orderId);

            return new Response<object>()
            {
                isError = false,
                Message = "Order is Canceled Successfully",
                Payload = details
            };
        }


        public async Task<Response<object>> RejectOrder(string orderId)
        {
            var spec = new OrderWithRequestsSpecification(orderId);
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
            if (order.OrderStatusID != "1") //if not pending
            {
                if (order.OrderStatusID == "2")
                {
                    return new Response<object>()
                    {
                        isError = true,
                        Payload = null,
                        Message = "Order is already approved",
                        Errors = new List<string>() { "Error with order" },

                    };
                }
                if (order.OrderStatusID == "3")
                {
                    return new Response<object>()
                    {
                        isError = true,
                        Payload = null,
                        Message = "Order is already rejected",
                        Errors = new List<string>() { "Error with order" },

                    };
                }
                return new Response<object>()
                {
                    isError = true,
                    Payload = null,
                    Message = "Order is either canceled or completed",
                    Errors = new List<string>() { "Error with order" },
                };
            }
            await _orderRepository.RejectOrder(order);
            _unitOfWork.Commit();

            var customer = order.Customer;
            var message = new EmailDto(customer.Email!, "Sarvicny: Rejected", "Unfortunally! Your order is Rejected ");  // akedd momkn yet7sn (n2ol feeh al details mslun)

            _emailService.SendEmail(message);

            ///ReAsignnnnnn??
            var details = await _orderService.ShowOrderDetailsForProvider(orderId);

            return new Response<object>()
            {
                isError = false,
                Message = "Order is Rejected Successfully",
                Payload = details
            };
        }


        public async Task<Response<List<object>>> getAllOrders(string workerId)
        {
            var spec = new OrderWithRequestsSpecification();
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
                if (order.ServiceRequests.Any(s => s.providerService.ProviderID == workerId))
                {
                    var orderDetails = await _orderService.ShowOrderDetailsForProvider(order.OrderID);

                    result.Add(orderDetails);
                }
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

        public async Task<Response<List<object>>> getAllApprovedOrders(string workerId)
        {
            var spec = new OrderWithRequestsSpecification();
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
                if (order.OrderStatusID == "2") //2 means approved 
                {
                    if (order.ServiceRequests.Any(s => s.providerService.ProviderID == workerId))
                    {
                        var orderDetails = await _orderService.ShowOrderDetailsForProvider(order.OrderID);

                        result.Add(orderDetails);

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
            var spec = new OrderWithRequestsSpecification();
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
                    if (order.ServiceRequests.Any(s => s.providerService.ProviderID == workerId))
                    {
                        var orderDetails = await _orderService.ShowOrderDetailsForProvider(order.OrderID);

                        result.Add(orderDetails);

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

        public async Task<Response<object>> getRegisteredServices(string providerId)
        {
            var response = new Response<object>();

            var provider =
                await _serviceProviderRepository.FindByIdAsync(
                    new ServiceProviderWithServiceSpecificationcs(providerId));
            if (provider == null)
            {
                response.isError = true;
                response.Status = "failed";
                response.Message = "Provider Not Found";
                response.Errors.Add("Provider Not Found");
                return response;
            }

            var services = provider.ProviderServices.Select(s => new
            {
                s.ServiceID,
                s.Service.ServiceName,
                s.Service.CriteriaID,
                criteriaName = s.Service.Criteria?.CriteriaName,
                s.Price
            }).ToList<object>();

            response.Status = "Success";
            response.Message = "Action Done Successfully";
            response.Payload = services;
            return response;

        }

        public async Task<Response<object>> ShowProviderProfile(string providerId)
        {
            var spec = new ProviderWithServicesAndAvailabilitiesSpecification(providerId);
            var serviceProvider = await _serviceProviderRepository.FindByIdAsync(spec);
            if (serviceProvider == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Provider Not Found",
                    Payload = null,
                    isError = true
                };
            }



            var services = serviceProvider.ProviderServices.Select(p => p.Service);
            var servicesAsObject = services.Select(s => new
            {
                s.ServiceID,
                s.ServiceName,
                s.ParentServiceID,
                parentServiceName = s.ParentService?.ServiceName,
                s.CriteriaID,
                s.Criteria?.CriteriaName,
            });

            dynamic profile = new ExpandoObject();

            profile.FirstName = serviceProvider.FirstName;
            profile.LastName = serviceProvider.LastName;
            profile.UserName = serviceProvider.UserName;
            profile.Email = serviceProvider.Email;
            profile.PhoneNumber = serviceProvider.PhoneNumber;
            profile.Services = servicesAsObject;

            var claims = await _userRepository.GetClaims(serviceProvider);

            var isWorker = claims.FirstOrDefault(c => c.Value == "Worker") ;
            var isConsultant = claims.FirstOrDefault(c => c.Value == "Consultant") ;
            var isCompany = claims.FirstOrDefault(c => c.Value == "Company") ;

            if (isWorker is not null)
            {
                Worker workerInstance = serviceProvider as Worker;
                profile.NationalID = workerInstance.NationalID;
                profile.CriminalRecord= workerInstance.CriminalRecord;
                
            }
            else if (isConsultant is not null)
            {
                Consultant consultant = serviceProvider as Consultant;

                profile.NationalID = consultant.NationalID;
                profile.CriminalRecord = consultant.CriminalRecord;
                profile.Salary = consultant.salary;


            }
            else if (isCompany is not null)
            {
                Company companyInstance = serviceProvider as Company;
                profile.License = companyInstance.license;
                
            }

            else
            {

                return new Response<object>()
                {
                    Payload = null,
                    isError = true,
                    Message = "false"

                };
            }


            return new Response<object>()
            {
                Payload = profile,
                isError = false,
                Message = "Success"

            };
        }

        
    }
}
