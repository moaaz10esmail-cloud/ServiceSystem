using Microsoft.AspNetCore.Mvc;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Shared.Common;
using ServicesSystem.Shared.DTOs.Services;

namespace ServicesSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ServicesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<ServiceDto>>>> GetAll()
    {
        var services = await _unitOfWork.Services.GetAllAsync();
        var serviceDtos = services.Select(s => new ServiceDto
        {
            Id = s.Id,
            Name = s.Name,
            NameAr = s.NameAr,
            Description = s.Description,
            DescriptionAr = s.DescriptionAr,
            BasePrice = s.BasePrice,
            EstimatedDuration = s.EstimatedDuration,
            ImageUrl = s.ImageUrl,
            IsActive = s.IsActive,
            CategoryId = s.CategoryId,
            CategoryName = s.Category?.Name ?? ""
        });

        return Ok(Result<IEnumerable<ServiceDto>>.Success(serviceDtos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<ServiceDto>>> GetById(Guid id)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(id);
        if (service == null)
        {
            return NotFound(Result<ServiceDto>.Failure("Service not found"));
        }

        var serviceDto = new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            NameAr = service.NameAr,
            Description = service.Description,
            DescriptionAr = service.DescriptionAr,
            BasePrice = service.BasePrice,
            EstimatedDuration = service.EstimatedDuration,
            ImageUrl = service.ImageUrl,
            IsActive = service.IsActive,
            CategoryId = service.CategoryId,
            CategoryName = service.Category?.Name ?? ""
        };

        return Ok(Result<ServiceDto>.Success(serviceDto));
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<Result<IEnumerable<ServiceDto>>>> GetByCategory(Guid categoryId)
    {
        var services = await _unitOfWork.Services.GetByCategoryAsync(categoryId);
        var serviceDtos = services.Select(s => new ServiceDto
        {
            Id = s.Id,
            Name = s.Name,
            NameAr = s.NameAr,
            Description = s.Description,
            DescriptionAr = s.DescriptionAr,
            BasePrice = s.BasePrice,
            EstimatedDuration = s.EstimatedDuration,
            ImageUrl = s.ImageUrl,
            IsActive = s.IsActive,
            CategoryId = s.CategoryId,
            CategoryName = s.Category?.Name ?? ""
        });

        return Ok(Result<IEnumerable<ServiceDto>>.Success(serviceDtos));
    }

    [HttpPost]
    public async Task<ActionResult<Result<ServiceDto>>> Create([FromBody] CreateServiceDto dto)
    {
        var service = new Service
        {
            Name = dto.Name,
            NameAr = dto.NameAr,
            Description = dto.Description,
            DescriptionAr = dto.DescriptionAr,
            BasePrice = dto.BasePrice,
            EstimatedDuration = dto.EstimatedDuration,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId
        };

        await _unitOfWork.Services.AddAsync(service);
        await _unitOfWork.SaveChangesAsync();

        var serviceDto = new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            NameAr = service.NameAr,
            Description = service.Description,
            DescriptionAr = service.DescriptionAr,
            BasePrice = service.BasePrice,
            EstimatedDuration = service.EstimatedDuration,
            ImageUrl = service.ImageUrl,
            IsActive = service.IsActive,
            CategoryId = service.CategoryId
        };

        return CreatedAtAction(nameof(GetById), new { id = service.Id }, Result<ServiceDto>.Success(serviceDto, "Service created successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> Delete(Guid id)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(id);
        if (service == null)
        {
            return NotFound(Result.Failure("Service not found"));
        }

        await _unitOfWork.Services.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return Ok(Result.Success("Service deleted successfully"));
    }
}
