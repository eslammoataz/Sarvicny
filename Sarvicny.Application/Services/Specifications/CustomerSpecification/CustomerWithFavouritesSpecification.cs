using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Specifications.CustomerSpecification
{
    public class CustomerWithFavouritesSpecification : BaseSpecifications<Customer>
    {
        public CustomerWithFavouritesSpecification()
        {
            AddInclude($"{nameof(Customer.Favourites)}");

        }

        public CustomerWithFavouritesSpecification(string customerId) : base(c => c.Id == customerId)
        {
            
            AddInclude($"{nameof(Customer.Favourites)}");


        }
    }
}
