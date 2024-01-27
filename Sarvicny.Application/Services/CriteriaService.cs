using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.NewFolder;
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
                AvailabilityStatus = s.AvailabilityStatus,
                ParentServiceID = s.ParentServiceID,
                Price = s.Price
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
            AvailabilityStatus = s.AvailabilityStatus,
            ParentServiceID = s.ParentServiceID,
            Price = s.Price
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

    public async Task<Response<Criteria>> AddServiceToCriteria(string criteriaId, string serviceId)
    {

        var spec = new CriteriaWithServicesSpecification(criteriaId);
        var criteria = await _criteriaRepository.GetCriteriaById(spec);

        if (criteria == null)
        {
            return new Response<Criteria>()
            {
                Status = "Fail",
                Message = "Criteria Not Found",
                Payload = null,
                isError = true,
            };
        }

        var spec1 = new ServiceWithCriteriaSpecification(serviceId);
        var service = await _serviceRepository.GetServiceById(spec1);

        if (service == null)
        {
            return new Response<Criteria>()
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
                return new Response<Criteria>()
                {
                    Status = "Fail",
                    Message = "Service is already found in this Criteria",
                    Payload = null,
                    isError = true,
                };
            }
            return new Response<Criteria>()
            {
                Status = "Fail",
                Message = "Service is already assigned to another criteria",
                Payload = null,
                isError = true,
            };
        }

        return new Response<Criteria>()
        {
            Message = "Sucess",
            Payload = await _criteriaRepository.AddServiceToCriteria(criteriaId, serviceId),
            isError = false,
        };



    }
}