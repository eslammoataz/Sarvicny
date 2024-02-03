using Microsoft.AspNetCore.Identity;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Email;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
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



        //private IQueryable<Provider> ApplySpecification(ISpecifications<Provider> spec)
        //{
        //    return SpecificationBuilder<Provider>.Build(_context.Provider, spec);
        //}
        public async Task<Provider> ApproveServiceProviderRegister(string providerId)
        {

            var provider = _context.Provider.FirstOrDefault(p => p.Id == providerId);


            provider.IsVerified = true;

          

            return provider;
        }

        public async Task<Provider> RejectServiceProviderRegister(string providerId)
        {
            var provider = _context.Provider.FirstOrDefault(p => p.IsVerified == false);


            _context.Provider.Remove(provider);

           
            return provider;
        }
    }
}
