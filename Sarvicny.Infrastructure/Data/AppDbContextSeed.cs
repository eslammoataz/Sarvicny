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
            var workerData_1 = new Worker
            {
                UserName = "WORKER",
                Email = "eslamelmoataz@gmail.com",
                FirstName = "WORKER",
                LastName = "WORKER",
                PhoneNumber = "WORKER",
                NationalID = "WORKER",
                CriminalRecord = "WORKER",
                IsVerified = true,
                Availabilities = new List<ProviderAvailability>()

            };

            var workerData_2 = new Worker
            {
                UserName = "WORKER2",
                Email = "eslamelmoataz@gmail.com",
                FirstName = "WORKER2",
                LastName = "WORKER2",
                PhoneNumber = "WORKER2",
                NationalID = "WORKER2",
                CriminalRecord = "WORKER2",
                IsVerified = true,
                Availabilities = new List<ProviderAvailability>()

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
                await userManager.CreateAsync(workerData_1, "Worker123#");
                await userManager.AddToRoleAsync(workerData_1, "ServiceProvider");
                await userManager.AddClaimAsync(workerData_1, new Claim("UserType", "Worker"));

                await userManager.CreateAsync(workerData_2, "Worker123#");
                await userManager.AddToRoleAsync(workerData_2, "ServiceProvider");
                await userManager.AddClaimAsync(workerData_2, new Claim("UserType", "Worker"));

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

            Service childService_1 = null;
            Service childService_2 = null;

            if (!context.Services.Any())
            {
                // Add parent service
                await context.Services.AddAsync(serviceData);
            }


            // Check if child services need to be added
            if (!context.Services.Any(s => s.ParentServiceID == serviceData.ServiceID))
            {
                // Create child services
                childService_1 = new Service
                {
                    ServiceName = "child service 1",
                    Description = "This is a test child service 1.",
                    ParentServiceID = serviceData.ServiceID, // Assign parent service ID
                };

                childService_2 = new Service
                {
                    ServiceName = "child service 2",
                    Description = "This is a test child service 2.",
                    ParentServiceID = serviceData.ServiceID, // Assign parent service ID
                };

                // Add child services
                await context.Services.AddAsync(childService_1);
                await context.Services.AddAsync(childService_2);
            }

            // Save changes to ensure parent service ID is generated
            await context.SaveChangesAsync();

            // Check if ProviderServices need to be added
            if (!context.ProviderServices.Any())
            {
                // Add provider services
                var providerService_1_forWorker1 = new ProviderService()
                {
                    ProviderID = workerData_1.Id,
                    ServiceID = childService_1.ServiceID, // Use child service ID
                    Price = 99.99M,
                    Provider = workerData_1,
                    Service = childService_1,
                };

                var providerService_2_forWorker1 = new ProviderService()
                {
                    ProviderID = workerData_1.Id,
                    ServiceID = childService_2.ServiceID, // Use child service ID
                    Price = 199.99M,
                    Provider = workerData_1,
                    Service = childService_2,
                };


                var providerService_1_forWorker2 = new ProviderService()
                {
                    ProviderID = workerData_2.Id,
                    ServiceID = childService_1.ServiceID, // Use child service ID
                    Price = 99.99M,
                    Provider = workerData_2,
                    Service = childService_1,
                };

                var providerService_2_forWorker2 = new ProviderService()
                {
                    ProviderID = workerData_2.Id,
                    ServiceID = childService_2.ServiceID, // Use child service ID
                    Price = 199.99M,
                    Provider = workerData_2,
                    Service = childService_2,
                };




                // Add provider services to context
                await context.ProviderServices.AddAsync(providerService_1_forWorker1);
                await context.ProviderServices.AddAsync(providerService_2_forWorker1);

                await context.ProviderServices.AddAsync(providerService_1_forWorker2);
                await context.ProviderServices.AddAsync(providerService_2_forWorker2);

                // Associate provider services with serviceData and workerData
                childService_1.ProviderServices.Add(providerService_1_forWorker1);
                workerData_1.ProviderServices.Add(providerService_1_forWorker1);

                childService_1.ProviderServices.Add(providerService_1_forWorker2);
                workerData_2.ProviderServices.Add(providerService_1_forWorker2);

                childService_2.ProviderServices.Add(providerService_2_forWorker1);
                workerData_1.ProviderServices.Add(providerService_2_forWorker1);

                childService_2.ProviderServices.Add(providerService_2_forWorker2);
                workerData_2.ProviderServices.Add(providerService_2_forWorker2);

                // Save changes to persist associations
                await context.SaveChangesAsync();
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

            ProviderAvailability providerAvailability_Worker1 = null;
            ProviderAvailability providerAvailability_Worker2 = null;

            if (!context.ProviderAvailabilities.Any())
            {
                providerAvailability_Worker1 = await serviceProviderRepository.AddAvailability(avail, new BaseSpecifications<Provider>());
                providerAvailability_Worker2 = await serviceProviderRepository.AddAvailability(avail, new BaseSpecifications<Provider>());

                workerData_1.Availabilities.Add(providerAvailability_Worker1);
                workerData_2.Availabilities.Add(providerAvailability_Worker2);
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
            var providerDistrictData_worker1 = new ProviderDistrict
            {
                ProviderID = workerData_1.Id,
                DistrictID = districtData.DistrictID
            };

            var providerDistrictData_worker2 = new ProviderDistrict
            {
                ProviderID = workerData_2.Id,
                DistrictID = districtData.DistrictID
            };

            await context.ProviderDistricts.AddAsync(providerDistrictData_worker1);
            await context.ProviderDistricts.AddAsync(providerDistrictData_worker2);

            await context.SaveChangesAsync();


            //Seed customer cart

            var requestedServices = new RequestedService
            {
                Services = new List<Service> { childService_1, childService_2 }
            };

            await context.RequestedServices.AddAsync(requestedServices);
            await context.SaveChangesAsync();

            var slots = context.Slots.Where(s => s.ProviderAvailabilityID == providerAvailability_Worker1.ProviderAvailabilityID).ToList();

            var totalPrice = context.ProviderServices.Sum(ps => ps.Price);

            var newRequest = new CartServiceRequest
            {
                RequestedDate = DateTime.Now.AddDays(1),
                Provider = workerData_1,
                ProviderID = workerData_1.Id,
                RequestedServices = requestedServices,
                providerDistrict = providerDistrictData_worker1,
                Slot = slots.FirstOrDefault(),
                SlotID = slots.FirstOrDefault().TimeSlotID,
                CartID = customerData.Cart.CartID,
                Cart = customerData.Cart,

                Price = totalPrice,
                ProblemDescription = "this is problem discriotfdasfasf worker 1",
                Address = "sample address",
            };


            context.CartServiceRequests.Add(newRequest);

            customerData.Cart.CartServiceRequests.Add(newRequest);
            await context.SaveChangesAsync();


            var slots_2 = context.Slots.Where(s => s.ProviderAvailabilityID == providerAvailability_Worker2.ProviderAvailabilityID).ToList();
            var newRequest_2 = new CartServiceRequest
            {
                RequestedDate = DateTime.Now.AddDays(1),
                Provider = workerData_2,
                ProviderID = workerData_2.Id,
                RequestedServices = requestedServices,
                providerDistrict = providerDistrictData_worker2,
                Slot = slots_2.FirstOrDefault(),
                SlotID = slots_2.FirstOrDefault().TimeSlotID,
                CartID = customerData.Cart.CartID,
                Cart = customerData.Cart,

                Price = totalPrice,
                ProblemDescription = "this is problem discriotfdasfasf worker 2",
                Address = "sample address",
            };


            context.CartServiceRequests.Add(newRequest_2);

            customerData.Cart.CartServiceRequests.Add(newRequest_2);
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
                    }
                };

                context.OrderStatuses.AddRange(orderStatuses);
                context.SaveChanges();
            }

        }
    }
}