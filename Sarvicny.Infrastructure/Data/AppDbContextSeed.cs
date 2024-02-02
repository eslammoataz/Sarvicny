using Microsoft.AspNetCore.Identity;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;


namespace Sarvicny.Infrastructure.Data
{
    public static class AppDbContextSeed
    {
        public static async Task SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            await SeedAdmin(userManager, context);
            await SeedRoles(roleManager);
            await SeedOrderStatus(context);
            await SeedData(context, userManager, roleManager);
        }

        private static async Task SeedAdmin(UserManager<User> userManager, AppDbContext context)
        {
            if (!context.Admins.Any())
            {
                Admin admin = new Admin
                {
                    Id = "1",
                    UserName = "admin@example.com",
                    NormalizedUserName = "ADMIN@EXAMPLE.COM",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(admin, "Admin123#");
            }
        }

        private static async Task SeedData(AppDbContext context , UserManager<User>userManager , RoleManager<IdentityRole>roleManager)
        {
            
            var workerData = new Worker
            {
                UserName = "WORKER",
                Email = "eslamelmoataz7@gmail.com",
                FirstName = "WORKER",
                LastName = "WORKER",
                PhoneNumber = "WORKER",
                NationalID = "WORKER",
                CriminalRecord = "WORKER"
            };
            
            // Seed Customer data
            var customerData = new Customer
            {
                UserName = "Customer11111",
                Email = "Customer11111@example.com",
                FirstName = "Customer1111",
                LastName = "Customer11111",
                PhoneNumber = "Customer",
                Address = "Customer"
            };
            
            await userManager.CreateAsync(workerData, "Worker123#");
            await userManager.CreateAsync(customerData, "Customer123#");
            
            await userManager.AddToRoleAsync(workerData, "ServiceProvider");
            await userManager.AddToRoleAsync(customerData, "Customer");


            // Seed Service data
            var serviceData = new Service
            {
                ServiceName = "Roof Painting",
                Description = "This is a test service.",
                Price = 99.99M,
                AvailabilityStatus = "Available"
            };
            
            // Seed Criteria data
            var criteriaData = new Criteria
            {
                CriteriaName = "Home Criteria",
                Description = "Home Criteria description"
            };

            //var providerService = new ProviderService()
            //{
            //    ProviderID = workerData.Id,
            //    ServiceID = serviceData.ServiceID,
            //    Price = 99.99M,
            //    Provider = workerData,
            //    Service = serviceData
            //};

            //workerData.ProviderServices.Add(providerService);

            //await context.ProviderServices.AddAsync(providerService);

            await context.Criterias.AddAsync(criteriaData);
            await context.Services.AddAsync(serviceData);
            
            await context.SaveChangesAsync();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var admin = new IdentityRole("Admin");
                await roleManager.CreateAsync(admin);
            }

            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                var customer = new IdentityRole("Customer");
                await roleManager.CreateAsync(customer);
            }

            if (!await roleManager.RoleExistsAsync("ServiceProvider"))
            {
                var worker = new IdentityRole("ServiceProvider");
                await roleManager.CreateAsync(worker);
            }
        }

        private static async Task SeedOrderStatus(AppDbContext context)
        {
            if (!context.OrderStatuses.Any())
            {
                var orderStatuses = new List<OrderStatus>
                {
                    new OrderStatus
                    {
                        OrderStatusID = "1",
                        StatusName = "Pending"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "2",
                        StatusName = "Approved"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "3",
                        StatusName = "Rejected"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "4",
                        StatusName = "Canceled"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "5",
                        StatusName = "Completed"
                    }
                };
                await context.OrderStatuses.AddRangeAsync(orderStatuses);
                await context.SaveChangesAsync();
            }
        }
    }
}