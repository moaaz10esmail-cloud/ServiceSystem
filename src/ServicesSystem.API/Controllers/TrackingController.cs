using Microsoft.AspNetCore.Mvc;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Domain.ValueObjects;
using ServicesSystem.Shared.Common;
using ServicesSystem.Shared.DTOs.Tracking;

namespace ServicesSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackingController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public TrackingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("request/{requestId}")]
    public async Task<ActionResult<Result<IEnumerable<RequestTrackingDto>>>> GetTrackingHistory(Guid requestId)
    {
        var trackings = await _unitOfWork.Trackings.GetByRequestIdAsync(requestId);
        var trackingDtos = trackings.Select(MapToDto);

        return Ok(Result<IEnumerable<RequestTrackingDto>>.Success(trackingDtos));
    }

    [HttpGet("request/{requestId}/latest")]
    public async Task<ActionResult<Result<RequestTrackingDto>>> GetLatestTracking(Guid requestId)
    {
        var tracking = await _unitOfWork.Trackings.GetLatestByRequestIdAsync(requestId);
        if (tracking == null)
        {
            return NotFound(Result<RequestTrackingDto>.Failure("No tracking found for this request"));
        }

        return Ok(Result<RequestTrackingDto>.Success(MapToDto(tracking)));
    }

    [HttpPost("request/{requestId}")]
    public async Task<ActionResult<Result<RequestTrackingDto>>> AddTracking(Guid requestId, [FromBody] CreateTrackingDto dto)
    {
        // Validate request exists
        var request = await _unitOfWork.Requests.GetByIdAsync(requestId);
        if (request == null)
        {
            return NotFound(Result<RequestTrackingDto>.Failure("Request not found"));
        }

        var tracking = new RequestTracking
        {
            RequestId = requestId,
            Status = dto.Status,
            Notes = dto.Notes,
            CurrentLocation = dto.Latitude.HasValue && dto.Longitude.HasValue
                ? new Location(dto.Latitude.Value, dto.Longitude.Value)
                : null
        };

        await _unitOfWork.Trackings.AddAsync(tracking);
        await _unitOfWork.SaveChangesAsync();

        var trackingDto = MapToDto(tracking);
        return CreatedAtAction(
            nameof(GetLatestTracking),
            new { requestId },
            Result<RequestTrackingDto>.Success(trackingDto, "Tracking update added successfully"));
    }

    private static RequestTrackingDto MapToDto(RequestTracking tracking)
    {
        return new RequestTrackingDto
        {
            Id = tracking.Id,
            Status = tracking.Status,
            Notes = tracking.Notes,
            CurrentLocation = tracking.CurrentLocation != null
                ? new LocationDto
                {
                    Latitude = tracking.CurrentLocation.Latitude,
                    Longitude = tracking.CurrentLocation.Longitude
                }
                : null,
            CreatedAt = tracking.CreatedAt,
            RequestId = tracking.RequestId
        };
    }
}
