using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

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

    public async Task<Response<ICollection<Criteria>>> GetAllCriteria()
    {
        var response = new Response<ICollection<Criteria>>();
        var spec = new BaseSpecifications<Criteria>();
        response.Payload = await _criteriaRepository.GetAllCriterias(spec);
        return response;
    }

    public async Task<Response<Criteria>> GetCriteriaById(string criteriaId)
    {
        var response = new Response<Criteria>();
        var spec = new BaseSpecifications<Criteria>(c=> c.CriteriaID == criteriaId);
        response.Payload = await _criteriaRepository.GetCriteriaById(spec);
        return response;
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
        var response = new Response<Criteria>();
        await _criteriaRepository.AddCriteriaAsync(newCriteria);
        _unitOfWork.Commit();
        response.Payload = newCriteria;
        return response;
    }

    public async Task<Response<object>> AddServiceToCriteria(string criteriaId, string serviceId)
    {
        var response = new Response<object>();
        var spec1 = new BaseSpecifications<Criteria>(c => c.CriteriaID == criteriaId);

        var criteria = await _criteriaRepository.GetCriteriaById(spec1);

        if (criteria == null)
        {
            response.isError = true;
            response.Status = "Failed";
            response.Errors.Add($"Criteria with ID {criteriaId} not found.");

            return response;
        }

        var spec = new BaseSpecifications<Service>(s => s.ServiceID == serviceId);
        var service = await _serviceRepository.GetServiceById(spec);

        if (service == null)
        {
            response.isError = true;
            response.Status = "Failed";
            response.Errors.Add($"Service with ID {serviceId} not found.");

            return response;
        }
        
        
        await _criteriaRepository.AddServiceToCriteria(criteriaId, serviceId);
        
        response.Status = "Success";
        response.Message = "Action Done Successfully";
        response.Payload = service;
        _unitOfWork.Commit();

        return response;
    }
}