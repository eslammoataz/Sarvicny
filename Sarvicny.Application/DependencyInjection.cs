using Microsoft.Extensions.DependencyInjection;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services;
using Sarvicny.Application.Services.Email;

namespace Sarvicny.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmailConfirmService, EmailConfirmService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ICriteriaService, CriteriaService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IServiceProviderService, ServiceProviderService>();
        return services;
    }

}