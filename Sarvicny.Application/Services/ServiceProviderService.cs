using System.Dynamic;
using System.Linq;
using MailKit.Search;
using Org.BouncyCastle.Asn1.Ocsp;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Specifications.NewFolder;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Payment;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using static Sarvicny.Domain.Entities.OrderDetails;

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
        private readonly IDistrictRepository _districtRepository;
        private readonly IEmailService _emailService;
        


        private IOrderService _orderService;




        public ServiceProviderService(IUserRepository userRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepository, IOrderRepository orderRepository, ICustomerRepository customerRepository, IOrderService orderService, IEmailService emailService, IDistrictRepository districtRepository)
        {
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _serviceProviderRepository = serviceProviderRepository;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _orderService = orderService;
            _emailService = emailService;
            _districtRepository = districtRepository;
        }
        public async Task<Response<object>> RegisterServiceAsync(string workerId, string serviceId, decimal price)
        {
            var spec1 = new ProviderWithServices_Districts_AndAvailabilitiesSpecification(workerId);

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
            if (worker.IsBlocked == true)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider may be blocked "
                };
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


        public async Task<Response<object>> AddAvailability(AvailabilityDto availabilityDto, string workerId)
        {

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
            if (provider.IsVerified == false || provider.IsBlocked==true)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider may be Not Verified, or blocked "
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

        public async Task<Response<object>> RemoveAvailability(string availabilityId, string providerId)
        {
            var spec = new ProviderWithAvailabilitesSpecification(providerId);
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
            if (provider.IsVerified == false || provider.IsBlocked == true)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider may be Not Verified, or blocked "
                };
            }
            var availabilities= provider.Availabilities;
            var availabilty= availabilities.FirstOrDefault(a=>a.ProviderAvailabilityID == availabilityId);
            if (availabilty ==null) 
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Availabilty Not Found"
                };

            }

            await _serviceProviderRepository.RemoveAvailability(availabilty);
            provider.Availabilities.Remove(availabilty);
            _unitOfWork.Commit();
            return new Response<object>()

            {
                isError = false,
                Payload = availabilty,
                Message = "provider availability is removed successfully"
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
            if (provider.IsVerified == false || provider.IsBlocked == true)
            {
                return new Response<List<object>>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider may be Not Verified, or blocked "
                };
            }
            var availability = await _serviceProviderRepository.getAvailability(spec);

           

            var avail = availability.Select(a => new
            {
                a.ProviderAvailabilityID,
                //a.AvailabilityDate,
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

        public async Task<AvailabilityTimeSlot> getOriginalSlot(RequestedSlot RequestedSlot, string providerId)
        {
            var spec = new ProviderWithAvailabilitesSpecification(providerId);

            var provider = await _serviceProviderRepository.FindByIdAsync(spec);

            var availability = provider.Availabilities.FirstOrDefault(a => a.DayOfWeek == RequestedSlot.DayOfWeek);
            if (availability == null)
                return null;

            var slot = availability.Slots.FirstOrDefault(s => s.StartTime == RequestedSlot.StartTime);
            if (slot == null)
                return null;

            return slot;


        }

       
        public async Task<Response<object>> ApproveOrder(string orderId)
        {

            var spec = new OrderWithDetailsSpecification(orderId);
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
            if (order.OrderStatus == OrderStatusEnum.Approved)
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

            var originalSlot = await getOriginalSlot(order.OrderDetails.RequestedSlot, order.OrderDetails.ProviderID);
            if (originalSlot != null)
            {
                originalSlot.isActive = false;
            }

           

            DateTime tomorrow = DateTime.Today.AddDays(1);
            var paymentExpiryDate = tomorrow;
            if (order.OrderDetails.RequestedSlot.RequestedDay == tomorrow)
            {
                paymentExpiryDate = DateTime.UtcNow.AddHours(6);
            }

            order.PaymentExpiryTime = paymentExpiryDate;
            order.ExpiryDate = null;
            _unitOfWork.Commit();

            
            var details = await _orderService.ShowAllOrderDetailsForProvider(orderId);


            var customer = order.Customer;

            var orderDetailsForCustomer = _orderService.GenerateOrderDetailsMessage(order);
            var message = new EmailDto(customer.Email!, "Sarvicny: Request Approved", $"Thank you for using our system! Your Request is approved. \n\nOrder Details:\n{orderDetailsForCustomer}");
            if (order.PaymentMethod == PaymentMethod.Paymob  || order.PaymentMethod == PaymentMethod.Paypal)
            {
                message.Body += $"\n\nPlease note: Proceed to pay on  the website or the application using {order.PaymentMethod} ,Notice that the ExpiryDate for payment is {paymentExpiryDate} ,otherwise your order will be canceled";
            }
           

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
            var spec = new OrderWithDetailsSpecification(orderId);
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
            if (order.OrderStatus != OrderStatusEnum.Approved)
            {
                if (order.OrderStatus == OrderStatusEnum.Canceled)
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
                    Message = "Order is not approved to be canceled",
                    Errors = new List<string>() { "Error with order" },

                };

            }

            await _orderRepository.CancelOrder(order);

            var originalSlot = await getOriginalSlot(order.OrderDetails.RequestedSlot, order.OrderDetails.ProviderID);
            if (originalSlot != null)
            {
                originalSlot.isActive = true;
            }
            order.PaymentExpiryTime = null;

            _unitOfWork.Commit();


            var details = await _orderService.ShowAllOrderDetailsForProvider(orderId);


            var customer = order.Customer;

            var orderDetailsForCustomer = _orderService.GenerateOrderDetailsMessage(order);
            var message = new EmailDto(customer.Email!, "Sarvicny: Request Canceled", $"Unfortunately! Your Request is Canceled. \n\nOrder Details:\n{orderDetailsForCustomer} , We will try to recommend you other providers shortly.");
            _emailService.SendEmail(message);

            return new Response<object>()

            {
                isError = false,
                Payload = details,
                Message = "Order Canceled Succesfully",


            };
        }


        public async Task<Response<object>> RejectOrder(string orderId)
        {
            var spec = new OrderWithDetailsSpecification(orderId);
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
            if (order.OrderStatus == OrderStatusEnum.Approved)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Order is already approved",
                    Errors = new List<string>() { "Error with order" },

                };

            }
            if (order.OrderStatus == OrderStatusEnum.Rejected)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Order is already Rejected",
                    Errors = new List<string>() { "Error with order" },

                };
            }

            await _orderRepository.RejectOrder(order);

            order.ExpiryDate = null;
            _unitOfWork.Commit();


            var details = await _orderService.ShowAllOrderDetailsForProvider(orderId);


            var customer = order.Customer;

            var orderDetailsForCustomer = _orderService.GenerateOrderDetailsMessage(order);
            var message = new EmailDto(customer.Email!, "Sarvicny: Request Rejected", $"Unfortunately! Your Request is Rejected. \n\nOrder Details:\n{orderDetailsForCustomer} , We will try to recommend you other providers shortly.");
            _emailService.SendEmail(message);
            
         

            return new Response<object>()

            {
                isError = false,
                Payload = details,
                Message = "Order Rejected Succesfully",


            };
        }


        public async Task<Response<List<object>>> getAllOrdersForProvider(string workerId)
        {

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

            var spec = new OrderWithDetailsSpecification();
            var orders = await _orderRepository.GetAllOrdersForProvider(spec,workerId);

            if(orders.Count()== 0) {

                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "No Orders Found",
                    Payload = null,
                    isError = true
                };
            }
            

            List<object> result = new List<object>();
            foreach (var order in orders)
            {
                var orderDetails = await _orderService.ShowAllOrderDetailsForProvider(order.OrderID);

                result.Add(orderDetails);
            }

            return new Response<List<object>>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = result,

            };

        }

        public async Task<Response<List<object>>> getAllApprovedOrderForProvider(string workerId)
        {
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
            var spec = new OrderWithDetailsSpecification();
            var orders = await _orderRepository.GetAllApprovedOrdersForProvider(spec, workerId);

            if (orders.Count() == 0)
            {

                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "No Approved Orders Found",
                    Payload = null,
                    isError = true
                };
            }

            List<object> result = new List<object>();
            foreach (var order in orders)
            {
                var orderDetails = await _orderService.ShowAllOrderDetailsForProvider(order.OrderID);

                result.Add(orderDetails);
            }

            return new Response<List<object>>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = result,

            };


           

        }

        public async Task<Response<List<object>>> getAllPendingOrderForProvider(string workerId)
        {
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
            var spec = new OrderWithDetailsSpecification();
            var orders = await _orderRepository.GetAllPendingOrdersForProvider(spec, workerId);

            if (orders.Count() == 0)
            {

                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "No Approved Orders Found",
                    Payload = null,
                    isError = true
                };
            }

            List<object> result = new List<object>();
            foreach (var order in orders)
            {
                var orderDetails = await _orderService.ShowAllOrderDetailsForProvider(order.OrderID);

                result.Add(orderDetails);
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
                    new ServiceProviderWithService_DistrictSpecificationcs(providerId));
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
            var spec = new ProviderWithServices_Districts_AndAvailabilitiesSpecification(providerId);
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

            var isWorker = claims.FirstOrDefault(c => c.Value == "Worker");
            var isConsultant = claims.FirstOrDefault(c => c.Value == "Consultant");
            var isCompany = claims.FirstOrDefault(c => c.Value == "Company");

            if (isWorker is not null)
            {
                Worker workerInstance = serviceProvider as Worker;
                profile.NationalID = workerInstance.NationalID;
                profile.CriminalRecord = workerInstance.CriminalRecord;

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

        public async Task<Response<object>> AddDistrictToProvider(string providerId, string districtID)
        {
            var spec = new ProviderWithDistrictsSpecification(providerId);
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
            if (serviceProvider.IsVerified == false || serviceProvider.IsBlocked == true)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider may be Not Verified, or blocked "
                };
            }
            var district = await _districtRepository.GetDistrictById(districtID);
            if (district == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "District Not Found",
                    Payload = null,
                    isError = true
                };
            }
            if (district.Availability == false)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "District is Not Available",
                    Payload = null,
                    isError = true
                };
            }
            if (serviceProvider.ProviderDistricts.Any(d => d.District == district))
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "District is already Found",
                    Payload = null,
                    isError = true
                };
            }
            var districts = new ProviderDistrict()
            {
                Provider = serviceProvider,
                ProviderID = providerId,
                District = district,
                DistrictID = district.DistrictID,
                enable = true

            };
            var providerDistrict = await _districtRepository.AddDistrictToProvider(districts);
            serviceProvider.ProviderDistricts.Add(providerDistrict);
            _unitOfWork.Commit();
            var DistrictAsObject = serviceProvider.ProviderDistricts.Select(d => new
            {

                d.ProviderDistrictID,
                d.enable,
                d.District.DistrictName,


            }).FirstOrDefault(p => p.ProviderDistrictID == providerDistrict.ProviderDistrictID);
            return new Response<object>()
            {
                Status = "success",
                Message = "District added succesfully to provider",
                Payload = DistrictAsObject,
                isError = false
            };

        }

      


        public async Task<Response<object>> DisableDistrictFromProvider(string providerId, string districtID)
        {

            var spec = new ProviderWithDistrictsSpecification(providerId);
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
            if (serviceProvider.IsVerified == false || serviceProvider.IsBlocked == true)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider may be Not Verified, or blocked "
                };
            }
            var district = await _districtRepository.GetDistrictById(districtID);
            if (district == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "District Not Found",
                    Payload = null,
                    isError = true
                };
            }
            var providerDistrict = serviceProvider.ProviderDistricts.FirstOrDefault(d => d.DistrictID == districtID);

            if (providerDistrict == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "District is not assigned to the provider",
                    Payload = null,
                    isError = true
                };
            }
            if(providerDistrict.enable!=true)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "District is already disabled to the provider",
                    Payload = null,
                    isError = true
                };

            }
            providerDistrict.enable = false;
            _unitOfWork.Commit();

            var RemovedDistrictAsObject = new
            {

                providerDistrict.ProviderDistrictID,
                providerDistrict.enable,
                district.DistrictName,


            };
            return new Response<object>()
            {
                Status = "success",
                Message = "District disabled succesfully from provider",
                Payload = RemovedDistrictAsObject,
                isError = false
            };

        }
        public async Task<Response<object>> EnableDistrictToProvider(string providerId, string districtID)
        {
            var spec = new ProviderWithDistrictsSpecification(providerId);
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
            if (serviceProvider.IsVerified == false || serviceProvider.IsBlocked == true)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider may be Not Verified, or blocked "
                };
            }
            var district = await _districtRepository.GetDistrictById(districtID);
            if (district == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "District Not Found",
                    Payload = null,
                    isError = true
                };
            }
            var providerDistrict = serviceProvider.ProviderDistricts.FirstOrDefault(d => d.DistrictID == districtID);

            if (providerDistrict == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "District is not assigned to the provider",
                    Payload = null,
                    isError = true
                };
            }
            if (providerDistrict.enable != false)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "District is already enabled to the provider",
                    Payload = null,
                    isError = true
                };

            }
            providerDistrict.enable = true;
            _unitOfWork.Commit();

            var EnabledDistrictAsObject = new
            {

                providerDistrict.ProviderDistrictID,
                providerDistrict.enable,
                district.DistrictName,


            };
            return new Response<object>()
            {
                Status = "success",
                Message = "District enabled succesfully to provider",
                Payload = EnabledDistrictAsObject,
                isError = false
            };
        }

        public async Task<Response<List<object>>> GetProviderDistricts(string providerId)
        {

            var spec = new ProviderWithDistrictsSpecification(providerId);
            //var spec = new BaseSpecifications<Provider>(p => p.Id == providerId);

            var serviceProvider = await _serviceProviderRepository.FindByIdAsync(spec);
           
            if (serviceProvider == null)
            {
                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "Provider Not Found",
                    Payload = null,
                    isError = true
                };

            }
            if (serviceProvider.IsVerified == false || serviceProvider.IsBlocked == true)
            {
                return new Response<List<object>>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Provider may be Not Verified, or blocked "
                };
            }

            var districts = serviceProvider.ProviderDistricts.Select(d => new
            {
                d.DistrictID,
                d.District.DistrictName,
                d.enable


            }).ToList<object>();

            if (districts.Any())
            {
                return new Response<List<object>>()
                {
                    Status = "success",
                    Payload = districts,
                    isError = false
                };
            }
            return new Response<List<object>>()
            {
                Status = "failed",
                Message = "No districts found to this provider",
                Payload = null,
                isError = true
            };
        }

        public async Task<Response<object>> SetOrderStatus(string orderId, OrderStatusEnum status)
        {
            var spec = new OrderWithDetailsSpecification(orderId);
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
            var paymentMethod = order.PaymentMethod;
            if( paymentMethod == PaymentMethod.Cash)
            {
                if(order.OrderStatus != OrderStatusEnum.Approved)
                {
                    return new Response<object>()

                    {
                        isError = true,
                        Payload = null,
                        Message = "Order is't Approved",
                        Errors = new List<string>() { "Error with order" },

                    };
                }
                
               


            }
            else
            {
                if(!order.IsPaid)
                {
                    return new Response<object>()

                    {
                        isError = true,
                        Payload = null,
                        Message = "Order is't paid",
                        Errors = new List<string>() { "Error with order" },

                    };
                }
            }
            if(status == order.OrderStatus)
            {

                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "OrderStatus is already set",
                    Errors = new List<string>() { "Error with order" },

                };
            }
            if (status != OrderStatusEnum.Start && status != OrderStatusEnum.Preparing && status != OrderStatusEnum.OnTheWay && status != OrderStatusEnum.InProgress && status != OrderStatusEnum.Done)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "OrderStatus not valid",
                    Errors = new List<string>() { "Error with order" },

                };
            }
            order.OrderStatus = status;
            _unitOfWork.Commit();

         
            return new Response<object>()
            {
                isError = false,
                Payload = order.OrderStatus,
                Message = "Action Done Successfully",
                Errors = null,
            };



        }
    }
}
