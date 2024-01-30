﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sarvicny.Infrastructure.Data;

#nullable disable

namespace Sarvicny.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240129205132_EditingColumns")]
    partial class EditingColumns
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Avaliabilities.TimeSlot", b =>
                {
                    b.Property<string>("TimeSlotID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time(6)");

                    b.Property<string>("ProviderAvailabilityID")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time(6)");

                    b.Property<bool?>("enable")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("TimeSlotID");

                    b.HasIndex("ProviderAvailabilityID");

                    b.ToTable("Slots");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Cart", b =>
                {
                    b.Property<string>("CartID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("CustomerID")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("LastChangeTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("CartID");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Criteria", b =>
                {
                    b.Property<string>("CriteriaID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("CriteriaName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("CriteriaID");

                    b.ToTable("Criterias", (string)null);
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Order", b =>
                {
                    b.Property<string>("OrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("CustomerID")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("OrderStatusID")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<decimal?>("TotalPrice")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("OrderID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("OrderStatusID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.OrderStatus", b =>
                {
                    b.Property<string>("OrderStatusID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("OrderStatusID");

                    b.ToTable("OrderStatuses");

                    b.HasData(
                        new
                        {
                            OrderStatusID = "1",
                            StatusName = "Set"
                        });
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.ProviderAvailability", b =>
                {
                    b.Property<string>("ProviderAvailabilityID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime?>("AvailabilityDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DayOfWeek")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ServiceProviderID")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("ProviderAvailabilityID");

                    b.HasIndex("ServiceProviderID");

                    b.ToTable("ProviderAvailabilities");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.ProviderService", b =>
                {
                    b.Property<string>("ProviderID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ServiceID")
                        .HasColumnType("varchar(255)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("ProviderID", "ServiceID");

                    b.HasIndex("ServiceID");

                    b.ToTable("ProviderServices");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Service", b =>
                {
                    b.Property<string>("ServiceID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("AvailabilityStatus")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("CriteriaID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("ParentServiceID")
                        .HasColumnType("varchar(255)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ServiceID");

                    b.HasIndex("CriteriaID");

                    b.HasIndex("ParentServiceID");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.ServiceRequest", b =>
                {
                    b.Property<string>("ServiceRequestID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime?>("AddedTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CartID")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("SlotID")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("providerServiceProviderID")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("providerServiceServiceID")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("ServiceRequestID");

                    b.HasIndex("CartID");

                    b.HasIndex("providerServiceProviderID", "providerServiceServiceID");

                    b.ToTable("ServiceRequests");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("Users", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.Admin", b =>
                {
                    b.HasBaseType("Sarvicny.Domain.Entities.Users.User");

                    b.ToTable("Admins", (string)null);
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.Customer", b =>
                {
                    b.HasBaseType("Sarvicny.Domain.Entities.Users.User");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("CartID")
                        .HasColumnType("varchar(255)");

                    b.HasIndex("CartID")
                        .IsUnique();

                    b.ToTable("Customers", (string)null);
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.ServicProviders.Provider", b =>
                {
                    b.HasBaseType("Sarvicny.Domain.Entities.Users.User");

                    b.Property<bool>("isVerified")
                        .HasColumnType("tinyint(1)");

                    b.ToTable("ServiceProviders", (string)null);
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.ServicProviders.Company", b =>
                {
                    b.HasBaseType("Sarvicny.Domain.Entities.Users.ServicProviders.Provider");

                    b.Property<string>("license")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.ToTable("Companies", (string)null);
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.ServicProviders.Worker", b =>
                {
                    b.HasBaseType("Sarvicny.Domain.Entities.Users.ServicProviders.Provider");

                    b.Property<string>("CriminalRecord")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NationalID")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.ToTable("Workers", (string)null);
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.ServicProviders.Consultant", b =>
                {
                    b.HasBaseType("Sarvicny.Domain.Entities.Users.ServicProviders.Worker");

                    b.ToTable("Consultants", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sarvicny.Domain.Entities.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Avaliabilities.TimeSlot", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.ProviderAvailability", "ProviderAvailability")
                        .WithMany("Slots")
                        .HasForeignKey("ProviderAvailabilityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProviderAvailability");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Order", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sarvicny.Domain.Entities.OrderStatus", "OrderStatus")
                        .WithMany()
                        .HasForeignKey("OrderStatusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("OrderStatus");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.ProviderAvailability", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.ServicProviders.Provider", "ServiceProvider")
                        .WithMany("Availabilities")
                        .HasForeignKey("ServiceProviderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceProvider");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.ProviderService", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.ServicProviders.Provider", "Provider")
                        .WithMany("ProviderServices")
                        .HasForeignKey("ProviderID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Sarvicny.Domain.Entities.Service", "Service")
                        .WithMany("ProviderServices")
                        .HasForeignKey("ServiceID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Provider");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Service", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Criteria", "Criteria")
                        .WithMany("Services")
                        .HasForeignKey("CriteriaID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sarvicny.Domain.Entities.Service", "ParentService")
                        .WithMany("ChildServices")
                        .HasForeignKey("ParentServiceID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Criteria");

                    b.Navigation("ParentService");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.ServiceRequest", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Cart", "Cart")
                        .WithMany("ServiceRequests")
                        .HasForeignKey("CartID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sarvicny.Domain.Entities.ProviderService", "providerService")
                        .WithMany()
                        .HasForeignKey("providerServiceProviderID", "providerServiceServiceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("providerService");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.Admin", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.User", null)
                        .WithOne()
                        .HasForeignKey("Sarvicny.Domain.Entities.Users.Admin", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.Customer", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Cart", "Cart")
                        .WithOne("Customer")
                        .HasForeignKey("Sarvicny.Domain.Entities.Users.Customer", "CartID");

                    b.HasOne("Sarvicny.Domain.Entities.Users.User", null)
                        .WithOne()
                        .HasForeignKey("Sarvicny.Domain.Entities.Users.Customer", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cart");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.ServicProviders.Provider", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.User", null)
                        .WithOne()
                        .HasForeignKey("Sarvicny.Domain.Entities.Users.ServicProviders.Provider", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.ServicProviders.Company", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.ServicProviders.Provider", null)
                        .WithOne()
                        .HasForeignKey("Sarvicny.Domain.Entities.Users.ServicProviders.Company", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.ServicProviders.Worker", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.ServicProviders.Provider", null)
                        .WithOne()
                        .HasForeignKey("Sarvicny.Domain.Entities.Users.ServicProviders.Worker", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.ServicProviders.Consultant", b =>
                {
                    b.HasOne("Sarvicny.Domain.Entities.Users.ServicProviders.Worker", null)
                        .WithOne()
                        .HasForeignKey("Sarvicny.Domain.Entities.Users.ServicProviders.Consultant", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Cart", b =>
                {
                    b.Navigation("Customer")
                        .IsRequired();

                    b.Navigation("ServiceRequests");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Criteria", b =>
                {
                    b.Navigation("Services");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.ProviderAvailability", b =>
                {
                    b.Navigation("Slots");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Service", b =>
                {
                    b.Navigation("ChildServices");

                    b.Navigation("ProviderServices");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.Customer", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Sarvicny.Domain.Entities.Users.ServicProviders.Provider", b =>
                {
                    b.Navigation("Availabilities");

                    b.Navigation("ProviderServices");
                });
#pragma warning restore 612, 618
        }
    }
}
