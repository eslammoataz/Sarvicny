using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services;
using Sarvicny.Application.Services.Email;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Infrastructure.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Infrastructure.Persistence
{
    public class ServiceProviderRepository : IServiceProviderRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly IUnitOfWork  unitOfWork;
        private readonly IEmailService _emailService;   
      

        public ServiceProviderRepository(UserManager<User> userManager, AppDbContext context, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            this.unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task AddProviderService(ProviderService workerservice)
        {
            _context.providerServices.Add(workerservice);
            unitOfWork.Commit();
        }

        public async Task<Worker> FindByIdAsync(string workerId)
        {
            return await _context.Users.FindAsync(workerId) as Worker;
        }

        public async Task<bool> WorkerExists(string workerId)
        {
            return await _context.Users.AnyAsync(u => u.Id == workerId && u is Worker);
        }
        public async Task<bool> IsServiceRegisteredForWorker(string workerId, string serviceId)
        {
            var worker = await _context.Users.FindAsync(workerId) as Worker;

            if (worker == null)
            {

                return false;
            }

            return worker.ProviderServices.Any(ws => ws.ServiceID == serviceId);
        }

        public async Task<ICollection<object>> GetRegisteredServices(string workerId)
        {
            var worker = await _context.Workers
               .Include(w => w.ProviderServices)
               .ThenInclude(ws => ws.Service)
               .FirstOrDefaultAsync(w => w.Id == workerId);
            if (worker == null)
            {
                throw new Exception("Provider Not Found");
            }
            var registeredServicesInfo = worker.ProviderServices
               .Select(ws => new
               {
                   ServiceID = ws.Service.ServiceID,
                   ServiceName = ws.Service.ServiceName
               })
               .ToList<object>();
            return registeredServicesInfo;
        }

        public async Task<string> AddAvailability(AvailabilityDto availabilityDto, string providerId)
        {
            var provider = _context.Provider.FirstOrDefault(w => w.Id == providerId);
            if (provider == null)
            {
                throw new Exception("Provider Not Found");

            }
            var availability = new ProviderAvailability
            {
                ServiceProviderID = providerId,
                DayOfWeek = availabilityDto.DayOfWeek,
                AvailabilityDate = DateTime.Now,
                ServiceProvider = provider
            };

            List<TimeSlot> timeSlots = await ConverttoTimeSlot(availabilityDto.Slots, availability);
            availability.Slots = timeSlots;
            _context.ProviderAvailabilities.Add(availability);

            provider.Availabilities.Add(availability);
            unitOfWork.Commit();
            return "Success";

        }

        public async Task<List<TimeSlot>> ConverttoTimeSlot(List<TimeRange> timeRanges, ProviderAvailability providerAvailability)

        {
            List<TimeSlot> timeSlots = new List<TimeSlot>();

            foreach (var timeRange in timeRanges)
            {
                var timeslot = new TimeSlot
                {
                    StartTime = TimeSpan.Parse(timeRange.startTime),
                    EndTime = TimeSpan.Parse(timeRange.endTime),
                    enable = true,
                    ProviderAvailabilityID = providerAvailability.ProviderAvailabilityID,
                    ProviderAvailability = providerAvailability
                };
                timeSlots.Add(timeslot);

            }

            return timeSlots;
        }

        public async Task<List<object>> getAvailability(string providerId)
        {
            var provider = _context.Provider
                            .Include(p => p.Availabilities)
                             .ThenInclude(a => a.Slots)
                              .FirstOrDefault(p => p.Id == providerId);

            if (provider == null)
            {
                throw new Exception("Provider Not Found");
            }
            List<object> availability = new List<object>();

            var avails = provider.Availabilities
                .SelectMany(a => a.Slots)
                .Select(slot => new
                {
                    slot.StartTime,
                    slot.EndTime
                })
                .ToList<object>();


            List<object> reponse = new List<object>();

            foreach (var providerAvail in provider.Availabilities)
            {

                List<object> slots = new List<object>();

                foreach (var avail in providerAvail.Slots)
                {
                    slots.Add(new
                    {
                        StartTime = avail.StartTime,
                        EndTime = avail.EndTime
                    });

                }

                var obj = new
                {
                    DayOfWeek = providerAvail.DayOfWeek,
                    Date = providerAvail.AvailabilityDate,
                    Slots = slots
                };
                reponse.Add(obj);
            }
            return reponse;


        }

        public async Task<object> ShowOrderDetails(string orderId)
        {
            var order = _context.Orders
              .Include(o => o.Customer)
              .ThenInclude(c => c.Cart)
              .ThenInclude(c => c.ServiceRequests)
              .ThenInclude(c => c.providerService)
              .FirstOrDefault(o => o.OrderID == orderId);
            if (order == null)
            {
                throw new Exception("Order Not Found");
            }
            var customer = order.Customer;

            var response = new
            {
                CustomerData = new
                {
                    customer.FirstName,
                    customer.Address,
                    // Add other customer properties as needed
                },
                ServiceData = customer.Cart.ServiceRequests.Select(s => new
                {
                    ServiceId = s.providerService.Service.ServiceName
                }).ToList<object>(),
            };
            return response;
        }

        public  async Task<string> ApproveOrder(string orderId)
        {
            var order = _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefault(o => o.OrderID == orderId);
            if (order == null)
            {
                throw new Exception("Order Not Found");

            }

            var customer = order.Customer;

            //al a7sn yeb2a enum bs ana 7alian bzwdha fe al db
            order.OrderStatusID = "2";
            order.OrderStatus = _context.OrderStatuses.FirstOrDefault(o => o.StatusName == "Approved");

           
            unitOfWork.Commit();

            var message = new EmailDto(customer.Email!, "Sarvicny: Approved", "Your order is approved seccessfully");

            _emailService.SendEmail(message);
            unitOfWork.Commit();
            return "Order is approved";
        }

        public async Task<string> RejectOrder(string orderId)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.OrderID == orderId);
            if (order == null)
            {
                throw new Exception("Order Not Found");
            }
            var customer = order.Customer;

            //al a7sn yeb2a enum bs ana 7alian bzwdha fe al db
            order.OrderStatusID = "3";
            order.OrderStatus = _context.OrderStatuses.FirstOrDefault(o => o.StatusName == "Rejected");
             unitOfWork.Commit ();

            var message = new EmailDto(customer.Email!, "Sarvicny: Rejected", "Your order is Rejected ");

            _emailService.SendEmail(message);
            unitOfWork.Commit();
            ///ReAsignnnnnn??
            return "Order is rejected";

        }

        public async  Task<string> CancelOrder(string orderId)
        {
            var order = _context.Orders
               .Include(o => o.Customer)
               .FirstOrDefault(o => o.OrderID == orderId);
            if (order == null)
            {
                throw new Exception("Order Not Found");

            }
            if (order.OrderStatusID == "2") //Approved
            {

                //al a7sn yeb2a enum bs ana 7alian bzwdha fe al db
                order.OrderStatusID = "4";
                order.OrderStatus = _context.OrderStatuses.FirstOrDefault(o => o.StatusName == "Canceled");
            }
            else
            {
                throw new Exception("Order was not originally approved to be Canceled");
            }
            var customer = order.Customer;

            var message = new EmailDto(customer.Email!, "Sarvicny: Canceled", "Unfortunally your order is canceled");

            _emailService.SendEmail(message);

            unitOfWork.Commit();

            ///ReAsignnnnnn??
            return "Order is canceled";

        }
    }
    
}
