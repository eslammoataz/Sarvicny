using Microsoft.AspNetCore.Mvc;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IServiceProviderService
    {
        Task<Response<ProviderService>> RegisterServiceAsync(string workerId, string serviceId, decimal Price); //done
        Task<Response<ICollection<object>>> GetRegisteredServices(string workerId); //done

        Task<Response<object>> AddAvailability(AvailabilityDto availabilityDto, string workerId); //done

        Task<Response<ICollection<object>>> getAvailability(string workerId); //done

        Task<Response<Object>> ShowOrderDetails(string orderId); //done //na2s 7ta fy elspecification

        Task<Response<object>> ApproveOrder(string orderId); //done
        Task<Response<object>> RejectOrder(string orderId); //done
        Task<Response<object>> CancelOrder(string orderId); //done
        Task<Response<ICollection<Object>>> GetAllServiceProviders(); //done
        Task<Response<object>> AddAvailabilitySlots(TimeSlotDto slotDto, string availabilityId);
        
        }
}
