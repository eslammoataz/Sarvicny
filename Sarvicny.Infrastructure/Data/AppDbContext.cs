using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        public DbSet<FavProvider> FavProviders { get; set; }

        public DbSet<District> Districts { get; set; }
        public DbSet<ProviderDistrict> ProviderDistricts { get; set; }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Criteria> Criterias { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<ProviderService> ProviderServices { get; set; }
        public DbSet<ProviderAvailability> ProviderAvailabilities { get; set; }
        public DbSet<AvailabilityTimeSlot> Slots { get; set; }

        public DbSet<RequestedSlot> RequestedSlots { get; set; }
        public DbSet<RequestedService> RequestedServices { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartServiceRequest> CartServiceRequests { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<OrderRating> OrderRatings { get; set; }

        public DbSet<TransactionPayment> TransactionPayment { get; set; }



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

            builder.Entity<AvailabilityTimeSlot>()
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

            builder.Entity<AvailabilityTimeSlot>()
                .HasOne(ts => ts.ProviderAvailability)
                .WithMany(pa => pa.Slots)
                .HasForeignKey(ts => ts.ProviderAvailabilityID)
                .OnDelete(DeleteBehavior.Cascade);



            builder.Entity<Customer>()
                   .HasOne(c => c.Cart)
                   .WithOne(c => c.Customer)
                   .HasForeignKey<Customer>(c => c.CartID);




            builder.Entity<CartServiceRequest>()
               .HasOne(c => c.Slot)
               .WithMany()
               .HasForeignKey(c => c.SlotID)
               .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Order>()
            .HasOne(o => o.CRate)
            .WithOne()
            .HasForeignKey<Order>(o => o.customerRatingId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>()
                .HasOne(o => o.PRate)
                .WithOne()
                .HasForeignKey<Order>(o => o.providerRatingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TransactionPayment>()
                .HasMany(tp => tp.OrderList)
                .WithOne(o => o.TransactionPayment)
                .HasForeignKey(o => o.TransactionPaymentId)
                .IsRequired();

            builder.Entity<Order>()
                .HasOne(o => o.TransactionPayment)
                .WithMany(tp => tp.OrderList)
                .HasForeignKey(o => o.TransactionPaymentId)
                .IsRequired();

            /* builder.Entity<Order>()
               .HasOne(o => o.OrderDetails)
               .WithOne(od => od.Order)
               .HasForeignKey<Order>(o => o.OrderDetailsId)
               .OnDelete(DeleteBehavior.Cascade);*/



        }


    }
}
