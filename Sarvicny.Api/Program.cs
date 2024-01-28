using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sarvicny.Application;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Infrastructure;
using Sarvicny.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

{
    // Add services to the container.

    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var Configuration = builder.Configuration;



    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            string jwtIssuer = Configuration.GetSection("Jwt:Issuer").Value;
            string jwtKey = Configuration.GetSection("Jwt:Key").Value;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        }).AddCookie(options =>
        {
            options.Cookie.Name = "MyCookie";
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.SlidingExpiration = true;
        });

    builder.Services.AddControllers().AddJsonOptions(options =>
    {
      //  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      //  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;  
    });


    builder.Services.AddSingleton(Configuration);
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(Configuration);

}

var app = builder.Build();
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var Services = scope.ServiceProvider;
            var userManager = Services.GetRequiredService<UserManager<User>>();
            var roleManager = Services.GetRequiredService<RoleManager<IdentityRole>>();
            var context = Services.GetRequiredService<AppDbContext>();
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();

            context.Database.EnsureCreated();

            // Check if any migrations are pending
            if (context.Database.GetPendingMigrations().Any())
            {
                //dbContext.Database.Migrate(); // Apply pending migrations
            }

            await AppDbContextSeed.SeedData(userManager, roleManager, context);
        }
        catch (Exception ex)
        {
            var LoggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var Logger = LoggerFactory.CreateLogger<Program>(); // Creating a logger for the Program class
            Logger.LogError(ex, ex.Message); // Logging the error
        }
    }
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}