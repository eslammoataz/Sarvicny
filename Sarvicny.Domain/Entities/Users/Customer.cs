using Sarvicny.Domain.Entities.Users.ServicProviders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sarvicny.Domain.Entities.Users
{
    public class Customer : User
    {


        public string? CartID { get; set; }

        [ForeignKey("CartID")]
        public Cart? Cart { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();


        public List<FavProvider>? Favourites { get; set; } = new List<FavProvider>();

        //public CustomerDistrict CustomerDistrict { get; set; }

        public string DistrictName { get; set; }
        public string Address { get; set; }






    }
}
