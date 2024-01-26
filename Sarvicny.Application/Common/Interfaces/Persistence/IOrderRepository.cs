﻿using Microsoft.EntityFrameworkCore;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderById(ISpecifications<Order> specifications);
        Task<object> ApproveOrder(ISpecifications<Order> spec);
        Task<object> RejectOrder(ISpecifications<Order> spec);
        Task<object> CancelOrder(ISpecifications<Order> spec);
        Task<object> ShowOrderDetails(ISpecifications<Order> spec);
    }

}
