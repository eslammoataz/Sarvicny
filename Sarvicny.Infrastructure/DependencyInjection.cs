using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sarvicny.Application.Common.Interfaces.Authentication;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Infrastructure.Authentication;
using Sarvicny.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Infrastructure.Persistence;
using Sarvicny.Application.Services.Abstractions;

namespace Sarvicny.Infrastructure;


public static class DependencyInjection
{
    public static object AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("constr").Value;

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );

        // Configure the Interfaces for the Identity
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>() // this adds the implementation of the interfaces
            .AddDefaultTokenProviders();


        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        services.AddScoped<IUserRepository , UserRepository>();
        services.AddScoped<IRoleRepository , RoleRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<ICriteriaRepository, CriteriaRepository>();
        services.AddScoped<IServiceProviderRepository , ServiceProviderRepository>();
        services.AddScoped<IOrderRepository, OrderRepositry>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        

        services.AddAuthentication(); // UserManager / SigninManager / RoleManager

        return services;
    }
}