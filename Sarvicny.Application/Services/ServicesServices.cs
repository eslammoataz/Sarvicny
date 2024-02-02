using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.ServiceSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services;

public class ServicesServices : IServicesService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceProviderRepository _serviceProviderRepository;
    public ServicesServices(IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepository)
    {
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
        _serviceProviderRepository = serviceProviderRepository;
    }

    public async Task<Response<List<object>>> GetAllServices()
    {
        var spec = new ServiceWithCriteriaSpecification();
        var services = await _serviceRepository.GetAllServices(spec);

        var servicesAsObjects = services.Select(s=>new
        {
            s.ServiceID,
            s.ServiceName,
            s.CriteriaID,
            s.Criteria?.CriteriaName,
            s.Criteria?.Description
        }).ToList<object>();

        var response = new Response<List<object>>();
        response.Status = "Success";
        response.Message = "Action Done Successfully";
        response.Payload = servicesAsObjects;

        return response;
    }

    public async Task<Response<Service>> GetServiceById(string serviceId)
    {
        var spec = new ServiceWithCriteriaSpecification(serviceId);
        var service = await _serviceRepository.GetServiceById(spec);
        var response = new Response<Service>();
        response.Status = "Success";
        response.Message = "Action Done Successfully";
        response.Payload = service;

        return response;
    }



    public async Task<Response<Service>> UpdateService(Service service)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Service>> DeleteService(string serviceId)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Service>> AddServiceAsync(Service newService)
    {
        if(newService.ParentServiceID != null)
        {
            var spec = new ServiceWithParentAndChilds_CriteriaSpecification(newService.ParentServiceID);
            var parent = await _serviceRepository.GetServiceById(spec);
            newService.ParentService = parent;
            parent.ChildServices.Add(newService);
            
        }
        await _serviceRepository.AddServiceAsync(newService);
        _unitOfWork.Commit();
        var response = new Response<Service>();

        response.Status = "Success";
        response.Message = "Action Done Successfully";
        response.Payload = newService;

        return response;
    }


    public async Task<Response<object>> GetAllWorkersForService(string serviceId)
    {
        var response = new Response<object>();

        var spec = new ServiceWithProvidersSpecification(serviceId);

        var service = await _serviceRepository.GetServiceById(spec);

        if (service == null)
        {
            response.Status = "Fail";
            response.Message = "Service Not Found";
            response.Payload = new List<object>();
            response.isError = false;
            return response;
        }


        var providers = service.ProviderServices.Select(p => new
        {
            p.Provider.Id,
            p.Provider.FirstName, p.Provider.LastName,
            p.Provider.Email,
            availabilities=p.Provider.Availabilities.Select(a => new {
                a.DayOfWeek,
               slots= a.Slots.Select(s => new {
                   s.StartTime, s.EndTime
               }).ToList<object>(),
            }).ToList<object>(),
            

        });
        if (providers==null)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "No Providers Found",
                Payload = null,
                isError = false,
            };
        }

        //var providers = new List<object>();

        //foreach (var p in service.ProviderServices)
        //{
        //    //var spec3 = new BaseSpecifications<Provider>(pr => pr.Id == p.ProviderID);
        //    //var provider = await _serviceProviderRepository.FindByIdAsync(spec3);


        //    var serviceProvidersAsObjects = new
        //    {
        //        provider.Id,
        //        provider.FirstName,
        //        provider.LastName,
        //        provider.Email,
        //        provider.PhoneNumber,
        //        provider.isVerified,
        //    };

        //    providers.Add(serviceProvidersAsObjects);
        //}



        return new Response<object>()
        {
            Message = "Action Done Successfully",
            Payload = providers,
            isError = false,
        };


    }

    public async Task<Response<object>> GetAllChildsForService(string serviceId)
    {
        var spec = new ServiceWithParentAndChilds_CriteriaSpecification(serviceId);
        var service= await _serviceRepository.GetServiceById(spec);

        var response = new Response<object>();

        if (service == null)
        {
            response.Status = "Fail";
            response.isError = true;
            response.Message = "Service Not found";
            response.Payload = null;

        }
        if (service.ChildServices.Count()==0)
        {
            response.Status = "Fail";
            response.isError = true;
            response.Message = "Service Has No Children";
            response.Payload = null;


        }

        var servicesAsObjects = new
        {
            serviceId=service.ServiceID,
            serviceName=service.ServiceName,
            criteriaID=service.CriteriaID,
            criteriaName=service.Criteria?.CriteriaName,

            parentServiceId=service.ParentServiceID,
            parentServiceName=service.ParentService?.ServiceName,

            children=service.ChildServices?.Select(ch=> new
            {
                childServiceID= ch.ServiceID,
                childServiceName=ch.ServiceName,

            }).ToList<object>()
        };

       
        response.Status = "Success";
        response.Message = "Action Done Successfully";
        response.Payload = servicesAsObjects;
        response.isError=false;


        return response;




    }
}