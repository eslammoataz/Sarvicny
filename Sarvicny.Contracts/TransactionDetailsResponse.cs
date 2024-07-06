using Sarvicny.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Contracts
{
    public class TransactionDetailsResponse
    {
        public TransactionPayment Transaction { get; set; }
        public List<object> Orders { get; set; }
    }
}
