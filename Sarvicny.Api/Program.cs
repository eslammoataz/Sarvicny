using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sarvicny.Api.Middlewares;
using Sarvicny.Application;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Infrastructure;
using Sarvicny.Infrastructure.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var Configuration = builder.Configuration;

    // allowing CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders("x-Authorization") // Expose custom headers if needed
                    .SetIsOriginAllowed(_ => true)
                    .WithHeaders("Content-Type"); // Allow Content-Type header
            });
    });



    //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //    .AddJwtBearer(options =>
    //    {
    //        string jwtIssuer = Configuration.GetSection("Jwt:Issuer").Value;
    //        string jwtKey = Configuration.GetSection("Jwt:Key").Value;
    //        options.TokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuer = true,
    //            ValidateAudience = true,
    //            ValidateLifetime = true,
    //            ValidateIssuerSigningKey = true,
    //            ValidIssuer = jwtIssuer,
    //            ValidAudience = jwtIssuer,
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    //        };
    //    }).AddCookie(options =>
    //    {
    //        options.Cookie.Name = "MyCookie";
    //        options.Cookie.SameSite = SameSiteMode.Lax;
    //        options.Cookie.HttpOnly = true;
    //        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    //        options.SlidingExpiration = true;
    //    });
    //builder.Services.AddAuthorization(options =>
    //{
    //    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    //    options.AddPolicy("User", policy => policy.RequireRole("User"));
    //    options.AddPolicy("Customer", policy => policy.RequireRole("Customer"));
    //});


    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigninKey"]))
        };
    });


    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
        options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                                .RequireAuthenticatedUser()
                                .Build();
    });






    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        //  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        //  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;  
    });


    builder.Services.AddSwaggerGen(swagger =>
    {
        //This is to generate the Default UI of Swagger Documentation
        swagger.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "ASP.NET 5 Web API",
            Description = " ITI Project"
        });
        //To Enable authorization using Swagger (JWT)    
        swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
        });
        swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                    });
    });



    builder.Services.AddSingleton(Configuration);
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(Configuration);
}

var app = builder.Build();
{
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var Services = scope.ServiceProvider;
            var userManager = Services.GetRequiredService<UserManager<User>>();
            var roleManager = Services.GetRequiredService<RoleManager<IdentityRole>>();
            var context = Services.GetRequiredService<AppDbContext>();
            var serviceProviderRepository = Services.GetRequiredService<IServiceProviderRepository>();

            context.Database.EnsureCreated();

            // Check if any migrations are pending
            if (context.Database.GetPendingMigrations().Any())
            {
                //dbContext.Database.Migrate(); // Apply pending migrations
            }

            await AppDbContextSeed.SeedData(userManager, roleManager, context, serviceProviderRepository);
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
    app.UseCors("CorsPolicy");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}