using Microsoft.AspNetCore.Mvc;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Enums;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Domain.ValueObjects;
using ServicesSystem.Shared.Common;
using ServicesSystem.Shared.DTOs.Requests;

namespace ServicesSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public RequestsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<RequestDto>>>> GetAll()
    {
        var requests = await _unitOfWork.Requests.GetAllAsync();
        var requestDtos = requests.Select(MapToDto);

        return Ok(Result<IEnumerable<RequestDto>>.Success(requestDtos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<RequestDto>>> GetById(Guid id)
    {
        var request = await _unitOfWork.Requests.GetByIdAsync(id);
        if (request == null)
        {
            return NotFound(Result<RequestDto>.Failure("Request not found"));
        }

        return Ok(Result<RequestDto>.Success(MapToDto(request)));
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<Result<IEnumerable<RequestDto>>>> GetByCustomer(Guid customerId)
    {
        var requests = await _unitOfWork.Requests.GetByCustomerAsync(customerId);
        var requestDtos = requests.Select(MapToDto);

        return Ok(Result<IEnumerable<RequestDto>>.Success(requestDtos));
    }

    [HttpGet("technician/{technicianId}")]
    public async Task<ActionResult<Result<IEnumerable<RequestDto>>>> GetByTechnician(Guid technicianId)
    {
        var requests = await _unitOfWork.Requests.GetByTechnicianAsync(technicianId);
        var requestDtos = requests.Select(MapToDto);

        return Ok(Result<IEnumerable<RequestDto>>.Success(requestDtos));
    }

    [HttpPost]
    public async Task<ActionResult<Result<RequestDto>>> Create([FromBody] CreateRequestDto dto)
    {
        // Validate service exists
        var service = await _unitOfWork.Services.GetByIdAsync(dto.ServiceId);
        if (service == null)
        {
            return BadRequest(Result<RequestDto>.Failure("Service not found"));
        }

        var request = new ServiceRequest
        {
            Description = dto.Description,
            CustomerLocation = new Address(
                dto.CustomerLocation.Street,
                dto.CustomerLocation.City,
                dto.CustomerLocation.State,
                dto.CustomerLocation.PostalCode,
                dto.CustomerLocation.Country,
                dto.CustomerLocation.AdditionalInfo
            ),
            ScheduledDate = dto.ScheduledDate,
            ServiceId = dto.ServiceId,
            TotalAmount = service.BasePrice,
            Status = RequestStatus.Pending,
            CustomerId = Guid.NewGuid() // TODO: Get from authenticated user
        };

        await _unitOfWork.Requests.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var requestDto = MapToDto(request);
        return CreatedAtAction(nameof(GetById), new { id = request.Id }, Result<RequestDto>.Success(requestDto, "Request created successfully"));
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<Result>> UpdateStatus(Guid id, [FromBody] UpdateRequestStatusDto dto)
    {
        var request = await _unitOfWork.Requests.GetByIdAsync(id);
        if (request == null)
        {
            return NotFound(Result.Failure("Request not found"));
        }

        if (!Enum.TryParse<RequestStatus>(dto.Status, out var status))
        {
            return BadRequest(Result.Failure("Invalid status"));
        }

        request.Status = status;

        if (status == RequestStatus.InProgress && !request.StartedAt.HasValue)
        {
            request.StartedAt = DateTime.UtcNow;
        }
        else if (status == RequestStatus.Completed && !request.CompletedAt.HasValue)
        {
            request.CompletedAt = DateTime.UtcNow;
        }
        else if (status == RequestStatus.Cancelled)
        {
            request.CancellationReason = dto.Reason;
        }
        else if (status == RequestStatus.Rejected)
        {
            request.RejectionReason = dto.Reason;
        }

        await _unitOfWork.Requests.UpdateAsync(request);
        await _unitOfWork.SaveChangesAsync();

        return Ok(Result.Success("Request status updated successfully"));
    }

    [HttpPut("{id}/assign/{technicianId}")]
    public async Task<ActionResult<Result>> AssignTechnician(Guid id, Guid technicianId)
    {
        var request = await _unitOfWork.Requests.GetByIdAsync(id);
        if (request == null)
        {
            return NotFound(Result.Failure("Request not found"));
        }

        var technician = await _unitOfWork.Users.GetByIdAsync(technicianId);
        if (technician == null || technician.Role != Domain.Enums.UserRole.Technician)
        {
            return BadRequest(Result.Failure("Invalid technician"));
        }

        request.TechnicianId = technicianId;
        request.Status = RequestStatus.Accepted;

        await _unitOfWork.Requests.UpdateAsync(request);
        await _unitOfWork.SaveChangesAsync();

        return Ok(Result.Success("Technician assigned successfully"));
    }

    private static RequestDto MapToDto(ServiceRequest request)
    {
        return new RequestDto
        {
            Id = request.Id,
            Description = request.Description,
            CustomerLocation = new AddressDto
            {
                Street = request.CustomerLocation.Street,
                City = request.CustomerLocation.City,
                State = request.CustomerLocation.State,
                PostalCode = request.CustomerLocation.PostalCode,
                Country = request.CustomerLocation.Country,
                AdditionalInfo = request.CustomerLocation.AdditionalInfo
            },
            Status = request.Status.ToString(),
            TotalAmount = request.TotalAmount,
            ScheduledDate = request.ScheduledDate,
            StartedAt = request.StartedAt,
            CompletedAt = request.CompletedAt,
            CustomerId = request.CustomerId,
            CustomerName = request.Customer?.FullName ?? "",
            TechnicianId = request.TechnicianId,
            TechnicianName = request.Technician?.FullName,
            ServiceId = request.ServiceId,
            ServiceName = request.Service?.Name ?? "",
            CreatedAt = request.CreatedAt
        };
    }
}
