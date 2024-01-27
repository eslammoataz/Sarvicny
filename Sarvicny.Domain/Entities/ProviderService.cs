﻿using Sarvicny.Domain.Entities.Users.ServicProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class ProviderService
    {
        public string ProviderID { get; set; }
        public string ServiceID { get; set; }

        public decimal Price { get; set; }

        [ForeignKey("ProviderID")]
        public Provider Provider { get; set; }

        [ForeignKey("ServiceID")]
        public Service Service { get; set; }

        public List<ServiceRequest> ServiceRequest { get; set; }
    }
}