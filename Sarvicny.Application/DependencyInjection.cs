using Microsoft.Extensions.DependencyInjection;
using Sarvicny.Application.Services;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Paymob;
using Sarvicny.Application.Services.Paypal;
using System.Text.Json.Serialization;

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
        services.AddScoped<IServicesService, ServicesServices>();
        services.AddScoped<IServiceProviderService, ServiceProviderService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IHandlePayment, HandlePayment>();
        services.AddScoped<IOrderService, OrderService>();

        //Payment Services
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IPaymobPaymentService, PaymobPaymentService>();
        services.AddScoped<IPaypalPaymentService, PaypalPaymentService>();

        services.AddMvc().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });


        return services;
    }

}