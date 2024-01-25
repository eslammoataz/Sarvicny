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
using Sarvicny.Domain.Specification;
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
        public async Task<Provider> FindByIdAsync(string workerId)
        {
            return _context.Provider.FirstOrDefault(p => p.Id == workerId);
        }
        public async Task<Provider> FindByIdWithSpecificationAsync(string workerId, ISpecifications<Provider> specifications)
        {
            return await ApplySpecification(specifications).FirstOrDefaultAsync(p=>p.Id== workerId);
        }

        public async Task<bool> WorkerExists(string workerId)
        {
            return await _context.Users.AnyAsync(u => u.Id == workerId && u is Worker);
        }
        public async Task<bool> IsServiceRegisteredForWorker(string workerId, string serviceId)
        {
            var worker = await _context.Users.FindAsync(workerId) as Worker;

            //if (worker == null)
            //{

            //    return false;
            //}

            return worker.ProviderServices.Any(ws => ws.ServiceID == serviceId);
        }

        public async Task<ICollection<object>> GetRegisteredServices(string workerId, ISpecifications<Provider> spec)
        {

            var worker = await ApplySpecification(spec).FirstOrDefaultAsync(w => w.Id == workerId);

            var registeredServicesInfo = worker.ProviderServices
               .Select(ws => new
               {
                   ServiceID = ws.Service.ServiceID,
                   ServiceName = ws.Service.ServiceName
               })
               .ToList<object>();
            return registeredServicesInfo;
        }



        public async Task<object> AddAvailability(AvailabilityDto availabilityDto, string providerId)
        {
            var provider = _context.Provider.FirstOrDefault(w => w.Id == providerId);
            //if (provider == null)
            //{
            //    throw new Exception("Provider Not Found");

            //}
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
           
            return availability;

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

        public async Task<List<object>> getAvailability(string providerId, ISpecifications<Provider> spec)
        {
            var provider = await ApplySpecification(spec).FirstOrDefaultAsync(p => p.Id == providerId);

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

        private IQueryable<Provider> ApplySpecification(ISpecifications<Provider> spec)
        {
            return SpecificationBuilder<Provider>.Build(_context.Provider, spec);
        }

        public async Task<object> AddAvailabilitySlots(TimeSlotDto slotDto, string availabilityId)
        {
            var availability = _context.ProviderAvailabilities.FirstOrDefault(a => a.ProviderAvailabilityID == availabilityId);

            var slot = new TimeSlot
            {
                ProviderAvailabilityID = availability.ProviderAvailabilityID,
                StartTime = slotDto.StartTime,
                EndTime = slotDto.EndTime,
                enable = true,

            };


            _context.Slots.Add(slot);
            return slot;

        }

        
    }
    
}
