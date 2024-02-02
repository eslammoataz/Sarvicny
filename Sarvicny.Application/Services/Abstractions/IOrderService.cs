using Sarvicny.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IOrderService
    {
        Task<Response<Object>> ShowOrderDetailsForProvider(string orderId);
        Task<Response<object>> ShowAllOrderDetails(string orderId);
    }
}
