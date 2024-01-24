using Microsoft.AspNetCore.Identity;

namespace Sarvicny.Domain.Entities.Users
{
    public abstract class User : IdentityUser
    {

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

    }
}
