using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Email;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Infrastructure.Persistence
{
    public class ServiceProviderRepository : IServiceProviderRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly IUnitOfWork unitOfWork;
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
            _context.ProviderServices.Add(workerservice);

        }
        public async Task<Provider> FindByIdAsync(ISpecifications<Provider> specifications)
        {
            return await ApplySpecification(specifications).FirstOrDefaultAsync();

        }


        //public async Task<Provider> FindByIdWithSpecificationAsync(string workerId, ISpecifications<Provider> specifications)
        //{
        //    return await ApplySpecification(specifications).FirstOrDefaultAsync(p => p.Id == workerId);
        //}

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

        public async Task<ICollection<object>> GetRegisteredServices(ISpecifications<Provider> specifications)
        {
            throw new NotImplementedException();
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



        public async Task<ProviderAvailability> AddAvailability(AvailabilityDto availabilityDto, ISpecifications<Provider> specifications)
        {
            var provider = await ApplySpecification(specifications).FirstOrDefaultAsync();

            var providerAvailability = new ProviderAvailability
            {
                ServiceProviderID = provider.Id,
                DayOfWeek = availabilityDto.DayOfWeek,
                AvailabilityDate = availabilityDto.AvailabilityDate,
                ServiceProvider = provider
            };

            List<TimeSlot> timeSlots = await ConvertToTimeSlot(availabilityDto.Slots, providerAvailability);
            providerAvailability.Slots = timeSlots;
            _context.ProviderAvailabilities.Add(providerAvailability);

            provider.Availabilities.Add(providerAvailability);

            return providerAvailability;

        }

        private async Task<List<TimeSlot>> ConvertToTimeSlot(List<TimeRange> timeRanges, ProviderAvailability providerAvailability)
        {
            List<TimeSlot> timeSlots = new List<TimeSlot>();

            foreach (var timeRange in timeRanges)
            {
                TimeSpan startTime = TimeSpan.Parse(timeRange.startTime);
                TimeSpan endTime = TimeSpan.Parse(timeRange.endTime);

                // Iterate over 1-hour intervals within the time range
                for (TimeSpan currentHour = startTime; currentHour < endTime; currentHour = currentHour.Add(TimeSpan.FromHours(1)))
                {
                    var timeSlot = new TimeSlot
                    {
                        StartTime = currentHour,
                        EndTime = currentHour.Add(TimeSpan.FromHours(1)),
                        enable = true,
                        ProviderAvailabilityID = providerAvailability.ProviderAvailabilityID,
                        ProviderAvailability = providerAvailability
                    };
                    timeSlots.Add(timeSlot);
                }
            }

            return timeSlots;
        }

        public async Task<List<ProviderAvailability>> getAvailability(ISpecifications<Provider> spec)
        {
            var provider = await ApplySpecification(spec).FirstOrDefaultAsync();
            return provider.Availabilities;
        }
        public async Task<List<TimeSlot>> getAvailabilitySlots(ISpecifications<ProviderAvailability> spec)
        {
            var avail = await ApplySpecificationA(spec).FirstOrDefaultAsync();
            return avail.Slots;
        }
        private IQueryable<ProviderAvailability> ApplySpecificationA(ISpecifications<ProviderAvailability> spec)
        {
            return SpecificationBuilder<ProviderAvailability>.Build(_context.ProviderAvailabilities, spec);
        }
        private IQueryable<Provider> ApplySpecification(ISpecifications<Provider> spec)
        {
            return SpecificationBuilder<Provider>.Build(_context.Provider, spec);
        }





        public async Task<ICollection<Provider>> GetProvidersRegistrationRequest(ISpecifications<Provider> spec)
        {
            return await ApplySpecification(spec).Where(p => p.IsVerified == false).ToListAsync();
        }

        public async Task<ICollection<Provider>> GetAllServiceProviders(ISpecifications<Provider> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }
    }

}
