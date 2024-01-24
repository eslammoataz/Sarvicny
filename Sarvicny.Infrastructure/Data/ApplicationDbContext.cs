//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;

//namespace WebApplication1.Data
//{
//    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
//    {

//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
//        {
//        }




//        protected override void OnModelCreating(ModelBuilder builder)
//        {
//            base.OnModelCreating(builder);

//            builder.Entity<IdentityUser>().ToTable("Users");
//            builder.Entity<IdentityRole>().ToTable("Roles");
//            builder.Entity<IdentityUserRole<string>>().ToTable("User_Role");
//            builder.Entity<IdentityUserClaim<string>>().ToTable("User_Claims");
//            builder.Entity<IdentityUserLogin<string>>().ToTable("User_Logins");

//            builder.Entity<IdentityRoleClaim<string>>().ToTable("Role_Claims");
//            builder.Entity<IdentityUserToken<string>>().ToTable("User_Tokens");

//            SeedRoles(builder);

//        }

//        private static void SeedRoles(ModelBuilder builder)
//        {
//            builder.Entity<IdentityRole>().HasData(
//                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
//                new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "USER" }
//                );

//        }

//    }
//}
