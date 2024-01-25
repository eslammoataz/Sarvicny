using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface ICustomerService
    {
        public Task<Response<string>> RequestService(RequestServiceDto requestServiceDto, string customerId);

        public Task<Response<string>> CancelRequestService(string customerId, string requestId);

        public Task<Response<List<object>>> GetCustomerCart(string customerId);

        public Task<Response<string>> OrderService(string customerId);
    }
}
