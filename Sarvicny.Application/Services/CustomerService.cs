using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.CustomerSpecification;
using Sarvicny.Application.Services.Specifications.NewFolder;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceProviderRepository _providerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository; 


        public CustomerService(IServiceProviderRepository providerRepository
            , IUnitOfWork unitOfWork, IServiceRepository serviceRepository, ICustomerRepository customerRepository,
            IUserRepository userRepository, IOrderRepository orderRepository)
        {
            _providerRepository = providerRepository;
            _unitOfWork = unitOfWork;
            _serviceRepository = serviceRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }

        // public async Task<Response<string>> RequestService(RequestServiceDto requestServiceDto, string customerId)
        // {
        //     var spec = new BaseSpecifications<Provider>(p => p.Id == requestServiceDto.ProviderId);
        //
        //     var provider = await _providerRepository.FindByIdAsync(spec);
        //
        //     if (provider == null)
        //     {
        //         return new Response<string> { isError = true, Message = "Provider Not Found" };
        //     }
        //
        //     var serviceSpec = new BaseSpecifications<Service>(s => s.ServiceID == requestServiceDto.ServiceId);
        //     var service = await _serviceRepository.GetServiceById(serviceSpec);
        //     
        //     if (service == null)
        //     {
        //         return new Response<string> { isError = true, Message = "Service Not Found" };
        //
        //     }
        //
        //     var providerService =
        //         provider.ProviderServices.SingleOrDefault(ps => ps.ServiceID == requestServiceDto.ServiceId);
        //
        //     if (providerService == null)
        //     {
        //         return new Response<string> { isError = true, Message = "This Worker is not Registered for the Service" };
        //     }
        //
        //     var customerSpec = new BaseSpecifications<Customer>(c => c.Id == customerId);
        //     var customer =  await _customerRepository.GetCustomerById(customerSpec);
        //
        //     if (customer == null)
        //     {
        //         return new Response<string> { isError = true, Message = "Customer Not Found" };
        //     }
        //
        //
        //     var providerAvailabilities = provider.Availabilities.Where(a => a.AvailabilityDate == requestServiceDto.RequestDay);
        //
        //     var slots = providerAvailabilities.SelectMany(p => p.Slots);
        //     var slotExist = slots.SingleOrDefault(s => s.TimeSlotID == requestServiceDto.SlotID);
        //     
        //     if (slotExist == null)
        //     {
        //         return new Response<string> { isError = true, Message = "Slot Not Found" };
        //     }
        //
        //     var newRequest = new ServiceRequest
        //     {
        //         AddedTime = DateTime.Now,
        //         providerService = providerService,
        //         Slot = slotExist,
        //         SlotID = slotExist.TimeSlotID,
        //         CartID = customer.Cart.CartID,
        //         Cart = customer.Cart
        //     };
        //     
        //     await _customerRepository.AddRequestToCart(newRequest);
        //     _unitOfWork.Commit();
        //     return new Response<string> { isError = false, Message = "Service Request is added successfully to the cart" };
        //
        // }
        public async Task<Response<string>> RequestService(RequestServiceDto requestServiceDto, string customerId)
        {
            requestServiceDto.RequestDay = requestServiceDto.RequestDay.Date;
            
            var spec = new ProviderWithServicesAndAvailabilitiesSpecification(requestServiceDto.ProviderId);
            var provider = await _providerRepository.FindByIdAsync(spec);

            if (provider == null)
                return new Response<string>
                {
                    isError = true,
                    Errors = new List<string> { "Provider Not Found" },
                    Status = "Error", 
                    Message = "Failed", 
                };

            var serviceSpec = new BaseSpecifications<Service>(s => s.ServiceID == requestServiceDto.ServiceId);
            var service = await _serviceRepository.GetServiceById(serviceSpec);

            if (service == null)
                return new Response<string>
                {
                    isError = true,
                    Errors = new List<string> { "Service Not Found" },
                    Status = "Error", 
                    Message = "Failed",
                };

            var providerService =
                provider.ProviderServices.SingleOrDefault(ps => ps.ServiceID == requestServiceDto.ServiceId);

            if (providerService == null)
                return new Response<string>
                {
                    isError = true,
                    Errors = new List<string> { "This Worker is not Registered for the Service" },
                    Status = "Error", 
                    Message = "Failed", 
                };

            var customerSpec = new CustomerWithCartSpecification(customerId);
            var customer = await _customerRepository.GetCustomerById(customerSpec);

            if (customer == null)
                return new Response<string>
                {
                    isError = true,
                    Errors = new List<string> { "Customer Not Found" },
                    Status = "Error",
                    Message = "Customer Not Found", 
                };
            
            var slots = provider.Availabilities.SelectMany(p => p.Slots);
            
            var slotExist = slots.SingleOrDefault(s => s.TimeSlotID == requestServiceDto.SlotID);

            if (slotExist == null)
                return new Response<string>
                {
                    isError = true,
                    Errors = new List<string> { "Error with Date or Slot" },
                    Status = "Error", 
                    Message = "An Error Occured",
                };

            if (customer.Cart is null)
            {
                customer.Cart = new Cart
                {
                    CustomerID = customer.Id,
                    LastChangeTime = DateTime.UtcNow,
                    Customer=customer
                };
            }

            var newRequest = new ServiceRequest
            {
                AddedTime = DateTime.UtcNow,
                providerService = providerService,
                Slot = slotExist,
                SlotID = slotExist.TimeSlotID,
                CartID = customer.Cart.CartID,
                Cart = customer.Cart
            };
            
            slotExist.enable = false;

            await _customerRepository.AddRequest(newRequest);
            _unitOfWork.Commit();

            return new Response<string>
                { isError = false, Message = "Service Request is added successfully to the cart" };
        }

        public async Task<Response<object>> CancelRequestService(string customerId, string requestId)
        {
           
            // var customer = context.Customers
            //   .Include(c => c.Cart)
            //    .ThenInclude(ca => ca.ServiceRequests)
            //     .FirstOrDefault(p => p.Id == customerId);
            // if (customer == null)
            // {
            //     return new Response<string> { isError = true, Message = "Customer Not Found" };
            // }
            //
            // var cart = context.Carts.FirstOrDefault(c => customerId == c.CustomerID);
            // if (cart == null)
            // {
            //     return new Response<string> { isError = true, Message = " This Customer has no Cart" };
            // }
            //
            // var request = context.ServiceRequests.FirstOrDefault(s => s.ServiceRequestID == requestId);
            // if (request == null)
            // {
            //     return new Response<string> { isError = true, Message = " Request Not found" };
            //
            // }
            // customer.Cart.ServiceRequests.Remove(request);
            // context.SaveChanges();
            // return new Response<string> { isError = false, Message = " Request is removed succesfully" };

            return null;
        }

        public async Task<Response<List<object>>> GetCustomerCart(string customerId)
        {
            // var customer = context.Customers
            //       .Include(c => c.Cart)
            //        .ThenInclude(ca => ca.ServiceRequests).
            //        ThenInclude(s=>s.providerService)
            //         .FirstOrDefault(p => p.Id == customerId);
            // if (customer == null)
            // {
            //     return new Response<List<object>>() { Payload = null, Message = "Customer is not found" };
            // }
            //
            // var cart = customer.Cart;
            // if (cart == null)
            // {
            //     return new Response<List<object>>() { Payload = null, Message = "Cart is not found" };
            // }
            //
            //
            //
            // var response = customer.Cart.ServiceRequests.Select(s => new 
            // {
            //     s.AddedTime,
            //     s.providerService.ServiceID,
            //     s.providerService.ProviderID
            // }).ToList<object>();
            //
            //
            // return new Response<List<object>>() { Payload = response, Message = "Success" };
            return null;
        }

        public async Task<Response<object>> OrderCart(string customerId)
        {
            var spec = new CustomerWithCartSpecification(customerId);
            var customer = await _customerRepository.GetCustomerById(spec);
            if (customer == null)
            {
                return new Response<object>() {
                    Payload = null,
                    Message = "Customer is not found",
                     isError = true,
                     Errors = new List<string> { "Error with customer" },
                };
            }
            var cart =customer.Cart;
            if(cart == null)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Cart is not found",
                    Errors = new List<string> { "Error with cart" },
                    isError = true

                };
            }
            var order = new Order
            {
                Customer = customer,
                CustomerID = customerId,
                OrderStatusID = "1", //value (status name)=set
                
            };
            var totalPrice =cart.ServiceRequests.Sum(s => s.providerService.Price);
            order.TotalPrice = totalPrice;
            var order1 = await _orderRepository.AddOrder(order);
            _unitOfWork.Commit();

            var result = new
            {
                order1.OrderID,
                order1.CustomerID,
                order1.OrderStatusID,
                order1.OrderStatus.StatusName,
                order1.TotalPrice,
             
            };


            return new Response<object>()
            {
                Payload = result ,
                Message = "Success",
                isError = false

            };
        }

    }
}