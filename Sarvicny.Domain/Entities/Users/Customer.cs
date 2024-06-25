using Sarvicny.Domain.Entities.Users.ServicProviders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sarvicny.Domain.Entities.Users
{
    public class Customer : User
    {
        public string Address { get; set; }
        public string? CartID { get; set; }

        [ForeignKey("CartID")]
        public Cart? Cart { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();


        public List<FavProvider>? Favourites { get; set; } = new List<FavProvider>();




    }
}
