using Microsoft.AspNetCore.Identity;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;
using System.Security.Claims;


namespace Sarvicny.Infrastructure.Data
{
    public static class AppDbContextSeed
    {
        public static async Task SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            AppDbContext context, IServiceProviderRepository serviceProviderRepository)
        {
            await SeedRoles(roleManager);
            await SeedOrderStatus(context);
            await SeedAdmin(userManager, context, roleManager);
            await SeedData(context, userManager, serviceProviderRepository);
        }

        private static async Task SeedAdmin(UserManager<User> userManager, AppDbContext context, RoleManager<IdentityRole> roleManager)
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
                await userManager.AddToRoleAsync(admin, "Admin");
                await userManager.AddClaimAsync(admin, new Claim("UserType", "Admin"));
            }
        }

        private static async Task SeedData(AppDbContext context, UserManager<User> userManager,
            IServiceProviderRepository serviceProviderRepository)

        {
            // Seed Worker data
            var workerData = new Worker
            {
                UserName = "WORKER",
                Email = "eslamelmoataz7@gmail.com",
                FirstName = "WORKER",
                LastName = "WORKER",
                PhoneNumber = "WORKER",
                NationalID = "WORKER",
                CriminalRecord = "WORKER",
                IsVerified = true,
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

            if (!context.Workers.Any())
            {
                var wallet = new ProviderWallet
                {
                    ProviderId = workerData.Id,
                    PendingBalance = 0.0m,
                    HandedBalance = 0.0m,


                };
                workerData.Wallet = wallet;
                workerData.WalletId = wallet.WalletId;
                await userManager.CreateAsync(workerData, "Worker123#");
                await userManager.AddToRoleAsync(workerData, "ServiceProvider");
                await userManager.AddClaimAsync(workerData, new Claim("UserType", "Worker"));

            }

            if (!context.Customers.Any())
            {
                var cart = new Cart
                {
                    CustomerID = customerData.Id,
                    Customer = customerData,
                    LastChangeTime = DateTime.Now
                };

                customerData.Cart = cart;
                customerData.CartID = cart.CartID;

                await userManager.CreateAsync(customerData, "Customer123#");
                await userManager.AddToRoleAsync(customerData, "Customer");
                await userManager.AddClaimAsync(customerData, new Claim("UserType", "Customer"));

            }

            // Seed Criteria data
            var criteriaData = new Criteria
            {
                CriteriaName = "Home Criteria",
                Description = "Home Criteria description"
            };

            if (!context.Criterias.Any())
            {
                await context.Criterias.AddAsync(criteriaData);
                await context.SaveChangesAsync();
            }


            // Seed Service data
            var serviceData = new Service
            {
                ServiceName = "Roof Painting",
                Description = "This is a test service.",
            };


            Service childService = null;

            if (!context.Services.Any())
            {
                if (context.Criterias.Any())
                    serviceData.CriteriaID = context.Criterias.FirstOrDefault()?.CriteriaID;

                await context.Services.AddAsync(serviceData);

                childService = new Service
                {
                    ServiceName = "child service",
                    Description = "This is a test child service.",
                    ParentServiceID = serviceData.ServiceID,
                };

                await context.Services.AddAsync(childService);

            }


            if (!context.ProviderServices.Any())
            {
                var providerService = new ProviderService()
                {
                    ProviderID = workerData.Id,
                    ServiceID = childService.ServiceID,
                    Price = 99.99M,
                    Provider = workerData,
                    Service = childService
                };
                await context.ProviderServices.AddAsync(providerService);

                childService.ProviderServices.Add(providerService);
                workerData.ProviderServices.Add(providerService);
            }

            //seed provider availability 
            var avail = new AvailabilityDto
            {
                DayOfWeek = "Saturday",
                //AvailabilityDate = DateTime.Now,
                Slots = new List<TimeRange>
                {
                    new TimeRange
                    {
                        startTime = "03:00:00",
                         endTime= "08:00:00"
                    }
                }
            };

            if (!context.ProviderAvailabilities.Any())
            {
                var providerAvailability = await serviceProviderRepository.AddAvailability(avail, new BaseSpecifications<Provider>());
                workerData.Availabilities.Add(providerAvailability);
            }

            //seed District data
            var districtData = new District
            {
                DistrictName = "Giza",
            };

            if (!context.Districts.Any())
            {
                await context.Districts.AddAsync(districtData);
                await context.SaveChangesAsync();
            }

            //seed ProviderDistrict data
            var providerDistrictData = new ProviderDistrict
            {
                ProviderID = workerData.Id,
                DistrictID = districtData.DistrictID
            };

            await context.ProviderDistricts.AddAsync(providerDistrictData);
            await context.SaveChangesAsync();


            //Seed customer cart

            //var requestedServices = new RequestedService
            //{
            //    Services = new List<Service> { childService }
            //};

            //await context.RequestedServices.AddAsync(requestedServices);
            await context.SaveChangesAsync();

            //var slots = context.Slots.ToList();

            //var newRequest = new CartServiceRequest
            //{
            //    RequestedDate = DateTime.Now.AddDays(1),
            //    Provider = workerData,
            //    ProviderID = workerData.Id,

            //    providerDistrict = providerDistrictData,
            //    Slot = slots.FirstOrDefault(),
            //    SlotID = slots.FirstOrDefault().TimeSlotID,
            //    CartID = customerData.Cart.CartID,
            //    Cart = customerData.Cart,

            //    Price = 100,
            //    ProblemDescription = "this is problem discriotfdasfasf",
            //    Address = "sample address",
            //};

            //newRequest.RequestedServices.Add(requestedServices);


            //context.CartServiceRequests.Add(newRequest);

            //customerData.Cart.CartServiceRequests.Add(newRequest);
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
                        StatusName = "Paid"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "3",
                        StatusName = "Start"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "4",
                        StatusName = "Preparing"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "5",
                        StatusName = "On The Way"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "6",
                        StatusName = "In Progress"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "7",
                        StatusName = "Done"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "8",
                        StatusName = "Completed"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "9",
                        StatusName = "Removed"
                    },
                     new OrderStatus
                    {
                        OrderStatusID = "10",
                        StatusName = "CanceledByProvider"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "11",
                        StatusName = "ReAssigned"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "12",
                        StatusName = "Canceled"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "13",
                        StatusName = "Refunded"
                    },
                    new OrderStatus
                    {
                        OrderStatusID = "14",
                        StatusName = "RemovedWithRefund"
                    }
                };

                context.OrderStatuses.AddRange(orderStatuses);
                context.SaveChanges();
            }

        }
    }
}