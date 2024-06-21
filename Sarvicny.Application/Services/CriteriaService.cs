using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.NewFolder;
using Sarvicny.Application.Services.Specifications.ServiceSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services;

public class CriteriaService : ICriteriaService
{
    private readonly ICriteriaRepository _criteriaRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CriteriaService(ICriteriaRepository criteriaRepository, IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork)
    {
        _criteriaRepository = criteriaRepository;
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Response<ICollection<Criteria>>> GetAllCriterias()
    {
        var spec = new CriteriaWithServicesSpecification();
        var criterias = await _criteriaRepository.GetAllCriterias(spec);

        if (criterias == null)
        {
            return new Response<ICollection<Criteria>>()
            {
                Message = "No Criterias Found",
                Payload = null,
                isError = true,
            };

        }

        foreach (var criteria in criterias)
        {
            criteria.Services = criteria.Services.Select(s => new Service
            {
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName,
                Description = s.Description,
                ParentServiceID = s.ParentServiceID
                
            }).ToList();
        }

        return new Response<ICollection<Criteria>>()
        {
            Message = "Sucess",
            Payload = criterias,
            isError = false,
        };

    }

    public async Task<Response<Criteria>> GetCriteriaById(string criteriaId)
    {
        var spec = new CriteriaWithServicesSpecification(criteriaId);
        var criteria = await _criteriaRepository.GetCriteriaById(spec);

        if (criteria == null)
        {
            return new Response<Criteria>()
            {
                Message = "No Criteria Found",
                Payload = null,
                isError = true,
            };
        }

        criteria.Services = criteria.Services.Select(s => new Service
        {
            ServiceID = s.ServiceID,
            ServiceName = s.ServiceName,
            Description = s.Description,
            ParentServiceID = s.ParentServiceID
            
        }).ToList();

        return new Response<Criteria>()
        {
            Message = "Sucess",
            Payload = criteria,
            isError = false,
        };

    }

    public async Task<Response<Criteria>> UpdateCriteria(Criteria criteria)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Criteria>> DeleteCriteria(string criteriaId)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Criteria>> AddCriteriaAsync(Criteria newCriteria)
    {
        var criteria = await _criteriaRepository.AddCriteriaAsync(newCriteria);
        _unitOfWork.Commit();

        return new Response<Criteria>()
        {
            Message = "Sucess",
            Payload = criteria,
            isError = false,
        };
    }

    public async Task<Response<object>> AddServiceToCriteria(string criteriaId, string serviceId)
    {

        var spec = new CriteriaWithServicesSpecification(criteriaId);
        var criteria = await _criteriaRepository.GetCriteriaById(spec);

        if (criteria == null)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Criteria Not Found",
                Payload = null,
                isError = true,
            };
        }

        var spec1 = new ServiceWithParentAndChilds_CriteriaSpecification(serviceId);
        var service = await _serviceRepository.GetServiceById(spec1);

        if (service == null)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Service Not Found",
                Payload = null,
                isError = true,
            };
        }

        if (service.Criteria != null)
        {
            if (service.Criteria == criteria)
            {
                return new Response<object>()
                {
                    Status = "Fail",
                    Message = "Service is already found in this Criteria",
                    Payload = null,
                    isError = true,
                };
            }
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Service is already assigned to another criteria",
                Payload = null,
                isError = true,
            };
        }

        await _criteriaRepository.AddServiceToCriteria(criteriaId, serviceId);

        if (service.ChildServices.Count() != 0) //law al service de 3ndha childs yeb2a kolhom ta7t al criteria de
        {
            foreach (var childService in service.ChildServices)
            {
                await _criteriaRepository.AddServiceToCriteria(criteriaId, childService.ServiceID);
            }

        }
        _unitOfWork.Commit();

        var criteriaAsObject = new
        {
            criteria.CriteriaID,
            criteria.CriteriaName,
            criteria.Description,

        };
        return new Response<object>()
        {
            Message = "Service and its child added successfully to criteria",
            Payload = criteriaAsObject,
            isError = false,
        };



    }
}