﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;


namespace Sarvicny.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Provider> Provider { get; set; }

        public DbSet<District> Districts { get; set; }
        public DbSet<ProviderDistrict> ProviderDistricts { get; set; }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Criteria> Criterias { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<ProviderService> ProviderServices { get; set; }
        public DbSet<ProviderAvailability> ProviderAvailabilities { get; set; }
        public DbSet<TimeSlot> Slots { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<CustomerRating> customerRatings { get; set; }
        public DbSet<ProviderRating> ProviderRatings { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the base User table
            builder.Entity<User>().ToTable("Users");

            builder.Entity<Provider>().ToTable("ServiceProviders");

            // Configure the Customer table
            builder.Entity<Customer>().ToTable("Customers");

            // Configure the ServiceProvider tables
            builder.Entity<Worker>().ToTable("Workers");
            builder.Entity<Consultant>().ToTable("Consultants");
            builder.Entity<Company>().ToTable("Companies");

            // Configure the Admin table
            builder.Entity<Admin>().ToTable("Admins");


            builder.Entity<Criteria>().ToTable("Criterias");


            //orderratingsdatabase
            /*   builder.Entity<OrderRating>().ToTable("OrderRatings");

               builder.Entity<OrderRating>().HasKey(R => R.RatingId);

               builder.Entity<OrderRating>().HasOne(c =>c.customer).
                 WithMany().HasForeignKey(c => c.CustomerId);


               builder.Entity<OrderRating>().HasOne(p => p.provider).
                WithMany().HasForeignKey(p => p.serviceProviderId);


               builder.Entity<OrderRating>().HasOne(o => o.order).
               WithMany().HasForeignKey( o=> o.OrderId);
            */
            /////////////////////////

            builder.Entity<Service>()
                .HasKey(s => s.ServiceID);


            builder.Entity<Service>()
                .HasOne(s => s.Criteria)
                .WithMany(s => s.Services)
                .HasForeignKey(s => s.CriteriaID)
                .OnDelete(DeleteBehavior.Restrict); // Choose the appropriate delete behavior

            builder.Entity<Service>()
                .HasMany(s => s.ChildServices)
                .WithOne(s => s.ParentService)
                .HasForeignKey(s => s.ParentServiceID)
                .OnDelete(DeleteBehavior.Restrict); // Choose the appropriate delete behavior

            // Define your relationships here using Fluent API

            builder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerID)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<ProviderAvailability>()
                .HasOne(wa => wa.ServiceProvider)
                .WithMany(w => w.Availabilities)
                .HasForeignKey(wa => wa.ServiceProviderID);

            builder.Entity<TimeSlot>()
                .HasOne(ts => ts.ProviderAvailability)
                .WithMany(pa => pa.Slots)
                .HasForeignKey(ts => ts.ProviderAvailabilityID);

            builder.Entity<ProviderService>()
       .HasKey(ws => ws.ProviderServiceID);

            builder.Entity<ProviderService>()
                .HasOne(ws => ws.Provider)
                .WithMany(w => w.ProviderServices)
                .HasForeignKey(ws => ws.ProviderID)
                .OnDelete(DeleteBehavior.Restrict); // Choose the appropriate delete behavior

            builder.Entity<ProviderService>()
                .HasOne(ws => ws.Service)
                .WithMany(s => s.ProviderServices)
                .HasForeignKey(ws => ws.ServiceID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProviderAvailability>()
               .HasOne(wa => wa.ServiceProvider)
               .WithMany(w => w.Availabilities)
               .HasForeignKey(wa => wa.ServiceProviderID);

            builder.Entity<TimeSlot>()
                .HasOne(ts => ts.ProviderAvailability)
                .WithMany(pa => pa.Slots)
                .HasForeignKey(ts => ts.ProviderAvailabilityID);

            builder.Entity<Customer>()
                   .HasOne(c => c.Cart)
                   .WithOne(c => c.Customer)
                   .HasForeignKey<Customer>(c => c.CartID);


            builder.Entity<ServiceRequest>()
                   .HasOne(sr => sr.Slot)
                   .WithMany()
                   .HasForeignKey(sr => sr.SlotID)
                   .OnDelete(DeleteBehavior.NoAction);

        }


    }
}
