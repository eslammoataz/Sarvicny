using Sarvicny.Domain.Entities.Users.ServicProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class ProviderWallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string WalletId { get; set; }

        public string ProviderId { get; set; }

      /*  [ForeignKey("providerId")]
        public Provider provider { get; set; }*/
        public decimal PendingBalance { get; set; }
        public decimal HandedBalance{ get; set; }


        public decimal TotalBalance
        {
            get
            {
                return PendingBalance - HandedBalance;
            }

            set {;}
        }

    }
}
