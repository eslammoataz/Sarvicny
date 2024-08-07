﻿using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sarvicny.Domain.Entities
{
    public class OrderDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OrderDetailsID { get; set; }


        public string OrderId { get; set; }

        public string ProviderID { get; set; }

        [ForeignKey("ProviderID")]
        public Provider Provider { get; set; }

        public List<RequestedService> RequestedServices { get; set; }

        public decimal Price { get; set; }

        public string RequestedSlotID { get; set; } = default!;

        [ForeignKey("RequestedSlotID")]
        public RequestedSlot RequestedSlot { get; set; }

        public string ProviderDistrictID { get; set; } = default!;

        [ForeignKey("ProviderDistrictID")]
        public ProviderDistrict providerDistrict { get; set; }

        public string Address { get; set; }


        [MaxLength(255)]
        public string? ProblemDescription { get; set; }


    }
}
