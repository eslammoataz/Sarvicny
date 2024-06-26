﻿using Microsoft.AspNetCore.Identity;
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
                ServiceProvider = provider,
                DayOfWeek = availabilityDto.DayOfWeek,



            };

            List<AvailabilityTimeSlot> timeSlots = await ConvertToTimeSlot(availabilityDto.Slots, providerAvailability);
            providerAvailability.Slots = timeSlots;

            _context.ProviderAvailabilities.Add(providerAvailability);

            return providerAvailability;

        }

        private async Task<List<AvailabilityTimeSlot>> ConvertToTimeSlot(List<TimeRange> timeRanges, ProviderAvailability providerAvailability)
        {
            List<AvailabilityTimeSlot> timeSlots = new List<AvailabilityTimeSlot>();

            foreach (var timeRange in timeRanges)
            {
                TimeSpan startTime = TimeSpan.Parse(timeRange.startTime);
                TimeSpan endTime = TimeSpan.Parse(timeRange.endTime);

                // Iterate over 1-hour intervals within the time range
                for (TimeSpan currentHour = startTime; currentHour < endTime; currentHour = currentHour.Add(TimeSpan.FromHours(1)))
                {
                    var timeSlot = new AvailabilityTimeSlot
                    {
                        StartTime = currentHour,
                        EndTime = currentHour.Add(TimeSpan.FromHours(1)),
                        isActive = true,
                        ProviderAvailabilityID = providerAvailability.ProviderAvailabilityID,
                        ProviderAvailability = providerAvailability

                    };
                    timeSlots.Add(timeSlot);
                }
            }

            return timeSlots;
        }

        public async Task RemoveAvailability(ProviderAvailability providerAvailability)
        {
            _context.ProviderAvailabilities.Remove(providerAvailability);
        }

        public async Task<List<ProviderAvailability>> getAvailability(ISpecifications<Provider> spec)
        {
            var provider = await ApplySpecification(spec).FirstOrDefaultAsync();
            return provider.Availabilities;
        }
        public async Task<List<AvailabilityTimeSlot>> getAvailabilitySlots(ISpecifications<ProviderAvailability> spec)
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
        public async Task<ICollection<Provider>> GetProvidersServiceRegistrationRequest(ISpecifications<Provider> spec)
        {
            return await ApplySpecification(spec).Where(p => p.IsVerified == true && p.ProviderServices.Any(ps => ps.isVerified == false)).ToListAsync();
        }
        public async Task<ICollection<Provider>> GetAllServiceProviders(ISpecifications<Provider> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }



        public async Task<List<Provider>> GetAllMatchedProviders(List<string> services, TimeSpan startTime, string dayOfWeek, string districtId, string customerId)
        {
            var initialProviders = await _context.Provider
                .Where(p => p.IsVerified && !p.IsBlocked)
                .Where(p => p.Availabilities.Any(a =>
                    a.DayOfWeek == dayOfWeek && a.Slots.Any(s =>
                    s.StartTime == startTime && s.isActive)))
                .Where(p => p.ProviderDistricts.Any(d => d.DistrictID == districtId && d.enable == true))
                .Include(p => p.ProviderServices)
                .ToListAsync();



            var matchedProviders = initialProviders
           .Where(p => services.All(serviceId =>
               p.ProviderServices.Where(ps => services.Contains(ps.ServiceID))
                   .Any(ps => ps.isVerified && ps.ServiceID == serviceId)))
           .ToList();


            var totalPricePerProvider = matchedProviders
                .Select(p => new
                {
                    Provider = p,
                    TotalPrice = p.ProviderServices
                        .Where(ps => services.Contains(ps.ServiceID))
                        .Sum(ps => ps.Price)
                })
                .ToList();


            var customer = await _context.Customers
                .Include(c => c.Favourites)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer != null && customer.Favourites.Count() != 0)
            {
                var favoriteProviderIds = customer.Favourites.Select(f => f.providerId).ToHashSet();

                matchedProviders = totalPricePerProvider
                       .OrderBy(pp => favoriteProviderIds.Contains(pp.Provider.Id) ? 0 : 1)
                       .ThenBy(pp => pp.TotalPrice)
                       .Select(pp => pp.Provider)
                       .ToList();
            }
            else
            {
                matchedProviders = totalPricePerProvider
                    .OrderBy(pp => pp.TotalPrice)
                    .Select(pp => pp.Provider)
                    .ToList();
            }

            return matchedProviders;
        }

        public async Task<List<Provider>> SuggestionLevel1(List<string> services, string dayOfWeek, string districtId, string customerId)
        {
            var initialProviders = await _context.Provider
               .Where(p => p.IsVerified && !p.IsBlocked)
               .Where(p => p.Availabilities.Any(a =>
                   a.DayOfWeek == dayOfWeek && a.Slots.Any(s => s.isActive)))
               .Where(p => p.ProviderDistricts.Any(d => d.DistrictID == districtId && d.enable == true))
               .Include(p => p.ProviderServices)
               .ToListAsync();


            var suggestedProviders = initialProviders
                .Where(p => services.All(serviceId =>
                   p.ProviderServices.Where(ps => services.Contains(ps.ServiceID))
                       .Any(ps => ps.isVerified && ps.ServiceID == serviceId)))
                .ToList();

            var totalPricePerProvider = suggestedProviders
                .Select(p => new
                {
                    Provider = p,
                    TotalPrice = p.ProviderServices
                        .Where(ps => services.Contains(ps.ServiceID))
                        .Sum(ps => ps.Price)
                })
                .ToList();

            var customer = await _context.Customers
                .Include(c => c.Favourites)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer != null && customer.Favourites.Count() != 0)
            {
                var favoriteProviderIds = customer.Favourites.Select(f => f.providerId).ToHashSet();

                suggestedProviders = totalPricePerProvider
           .OrderBy(pp => favoriteProviderIds.Contains(pp.Provider.Id) ? 0 : 1)
           .ThenBy(pp => pp.TotalPrice)
           .Select(pp => pp.Provider)
           .ToList();
            }

            else
            {
                suggestedProviders = totalPricePerProvider
                    .OrderBy(pp => pp.TotalPrice)
                    .Select(pp => pp.Provider)
                    .ToList();
            }

            return suggestedProviders;
        }

        public async Task<List<Provider>> SuggestionLevel2(List<string> services, string districtId, string customerId)
        {
            var initialProviders = await _context.Provider
              .Where(p => p.IsVerified && !p.IsBlocked)
              .Where(p => p.Availabilities.Any(a => a.Slots.Any(s => s.isActive)))
              .Where(p => p.ProviderDistricts.Any(d => d.DistrictID == districtId && d.enable == true))
              .Include(p => p.ProviderServices)
              .ToListAsync();

            //var suggestedProviders = initialProviders
            //    .Where(p => services.All(id =>
            //        p.ProviderServices.Any(ps => ps.isVerified == true && ps.ServiceID == id)))
            //    .ToList();

            var suggestedProviders = initialProviders
           .Where(p => services.All(serviceId =>
              p.ProviderServices.Where(ps => services.Contains(ps.ServiceID))
                  .Any(ps => ps.isVerified && ps.ServiceID == serviceId)))
           .ToList();

            var totalPricePerProvider = suggestedProviders
                .Select(p => new
                {
                    Provider = p,
                    TotalPrice = p.ProviderServices
                        .Where(ps => services.Contains(ps.ServiceID))
                        .Sum(ps => ps.Price)
                })
                .ToList();

            var customer = await _context.Customers
                .Include(c => c.Favourites)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer != null && customer.Favourites.Count() != 0)
            {
                var favoriteProviderIds = customer.Favourites.Select(f => f.providerId).ToHashSet();

                suggestedProviders = totalPricePerProvider
                         .OrderBy(pp => favoriteProviderIds.Contains(pp.Provider.Id) ? 0 : 1)
                         .ThenBy(pp => pp.TotalPrice)
                         .Select(pp => pp.Provider)
                         .ToList();
            }
            else
            {
                suggestedProviders = totalPricePerProvider
                    .OrderBy(pp => pp.TotalPrice)
                    .Select(pp => pp.Provider)
                    .ToList();
            }

            return suggestedProviders;
        }
    }

}
