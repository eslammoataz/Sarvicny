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

        //list<worker> favourite




    }
}
