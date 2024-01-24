using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Sarvicny.Domain.Entities.Users;


namespace Sarvicny.Application.Services.Email
{
    public class EmailConfirmService : IEmailConfirmService
    {
        private readonly UserManager<User> userManager;
        private readonly IUrlHelperFactory urlHelperFactory;
        private readonly IServiceProvider _serviceProvider;

        public EmailConfirmService(UserManager<User> userManager, IUrlHelperFactory urlHelperFactory, IServiceProvider serviceProvider)
        {
            this.userManager = userManager;
            this.urlHelperFactory = urlHelperFactory;
            _serviceProvider = serviceProvider;


        }


        public async Task<bool> ConfirmEmailAsync(string token, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                return result.Succeeded;
            }
            return false;
        }

        public string GenerateConfirmationLink(string token, string email, HttpContext httpContext = null)
        {
            if (httpContext == null)
            {
                var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
                httpContext = httpContextAccessor.HttpContext ?? new DefaultHttpContext();
            }
        
            var urlHelperFactory = _serviceProvider.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(new ActionContext(httpContext, new RouteData(), new ActionDescriptor()));
        
            var confirmationLink = urlHelper.Action("ConfirmEmail", "Authentication", new { token, email }, httpContext.Request.Scheme);
        
            return confirmationLink;
        }
    }
}



