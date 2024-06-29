using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Email;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Infrastructure.Persistence
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailService _emailService;

        public AdminRepository(UserManager<User> userManager, AppDbContext context, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            this.unitOfWork = unitOfWork;
            _emailService = emailService;
        }



        private IQueryable<Provider> ApplySpecification(ISpecifications<Provider> spec)
        {
            return SpecificationBuilder<Provider>.Build(_context.Provider, spec);
        }
        public async Task<Provider> ApproveServiceProviderRegister(ISpecifications<Provider> spec)
        {

            var provider = await ApplySpecification(spec).FirstOrDefaultAsync();


            provider.IsVerified = true;

            provider.ProviderServices.ForEach(provider => { provider.isVerified= true; });

          

            return provider;
        }

        public async Task ApproveProviderService(string providerServiceID)
        {

            var providerService= _context.ProviderServices.FirstOrDefault(p => p.ProviderServiceID == providerServiceID);

            providerService.isVerified= true;

        }
        public async Task RejectProviderService(string providerServiceID)
        {

            var providerService = _context.ProviderServices.FirstOrDefault(p => p.ProviderServiceID == providerServiceID);

            _context.ProviderServices.Remove(providerService);

        }
        public async Task<Provider> RejectServiceProviderRegister(string providerId)
        {
            var provider = _context.Provider.FirstOrDefault(p => p.IsVerified == false);


            _context.Provider.Remove(provider);

           
            return provider;
        }

    }
}
