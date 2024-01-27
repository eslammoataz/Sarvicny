using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Email;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using Sarvicny.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var provider = _context.Provider.FirstOrDefault(p=>p.Id==providerId);


            provider.isVerified = true;
            
            unitOfWork.Commit();

            //Add Token to Verify the email....
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(provider);


            var message = new EmailDto(provider.Email!, "Sarvicny: Worker Approved Successfully", "Congratulations you are accepted");

            _emailService.SendEmail(message);
           
            return provider;
        }

       public async Task<Provider> RejectServiceProviderRegister(string providerId)
        {
            var provider = _context.Provider.FirstOrDefault(p=> p.isVerified == false);


            _context.Provider.Remove(provider);
            
            unitOfWork.Commit();

            //Add Token to Verify the email....
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(provider);


            var message = new EmailDto(provider.Email!, "Sarvicny: Worker Rejected", "Sorry you are rejected");

            _emailService.SendEmail(message);

            return provider;
        }
    }
}
