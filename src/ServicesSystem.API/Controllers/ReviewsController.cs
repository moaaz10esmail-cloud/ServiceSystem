using Microsoft.AspNetCore.Mvc;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Enums;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Shared.Common;
using ServicesSystem.Shared.DTOs.Reviews;

namespace ServicesSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<ActionResult<Result<ReviewDto>>> CreateReview([FromBody] CreateReviewDto dto)
    {
        // Validate request exists and is completed
        var request = await _unitOfWork.Requests.GetByIdAsync(dto.RequestId);
        if (request == null)
        {
            return NotFound(Result<ReviewDto>.Failure("Request not found"));
        }

        if (request.Status != RequestStatus.Completed)
        {
            return BadRequest(Result<ReviewDto>.Failure("Can only review completed requests"));
        }

        // Check if review already exists for this request
        var existingReview = await _unitOfWork.Reviews.GetByRequestIdAsync(dto.RequestId);
        if (existingReview != null)
        {
            return BadRequest(Result<ReviewDto>.Failure("Review already exists for this request"));
        }

        // Validate rating range
        if (dto.Rating < 1 || dto.Rating > 5)
        {
            return BadRequest(Result<ReviewDto>.Failure("Rating must be between 1 and 5"));
        }

        var review = new Review
        {
            RequestId = dto.RequestId,
            CustomerId = request.CustomerId,
            TechnicianId = request.TechnicianId ?? Guid.Empty,
            Rating = dto.Rating,
            Comment = dto.Comment,
            IsPublic = true
        };

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        var reviewDto = MapToDto(review);
        return CreatedAtAction(
            nameof(GetById),
            new { id = review.Id },
            Result<ReviewDto>.Success(reviewDto, "Review created successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<ReviewDto>>> GetById(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
        {
            return NotFound(Result<ReviewDto>.Failure("Review not found"));
        }

        return Ok(Result<ReviewDto>.Success(MapToDto(review)));
    }

    [HttpGet("request/{requestId}")]
    public async Task<ActionResult<Result<ReviewDto>>> GetByRequestId(Guid requestId)
    {
        var review = await _unitOfWork.Reviews.GetByRequestIdAsync(requestId);
        if (review == null)
        {
            return NotFound(Result<ReviewDto>.Failure("Review not found for this request"));
        }

        return Ok(Result<ReviewDto>.Success(MapToDto(review)));
    }

    [HttpGet("technician/{technicianId}")]
    public async Task<ActionResult<Result<IEnumerable<ReviewDto>>>> GetByTechnicianId(Guid technicianId)
    {
        var reviews = await _unitOfWork.Reviews.GetByTechnicianIdAsync(technicianId);
        var reviewDtos = reviews.Select(MapToDto);

        return Ok(Result<IEnumerable<ReviewDto>>.Success(reviewDtos));
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<Result<IEnumerable<ReviewDto>>>> GetByCustomerId(Guid customerId)
    {
        var reviews = await _unitOfWork.Reviews.GetByCustomerIdAsync(customerId);
        var reviewDtos = reviews.Select(MapToDto);

        return Ok(Result<IEnumerable<ReviewDto>>.Success(reviewDtos));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateReview(Guid id, [FromBody] UpdateReviewDto dto)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
        {
            return NotFound(Result.Failure("Review not found"));
        }

        // Validate rating range
        if (dto.Rating < 1 || dto.Rating > 5)
        {
            return BadRequest(Result.Failure("Rating must be between 1 and 5"));
        }

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        review.IsPublic = dto.IsPublic;

        await _unitOfWork.Reviews.UpdateAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return Ok(Result.Success("Review updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteReview(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
        {
            return NotFound(Result.Failure("Review not found"));
        }

        await _unitOfWork.Reviews.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return Ok(Result.Success("Review deleted successfully"));
    }

    private static ReviewDto MapToDto(Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            IsPublic = review.IsPublic,
            RequestId = review.RequestId,
            CustomerId = review.CustomerId,
            CustomerName = review.Customer?.FullName ?? "",
            TechnicianId = review.TechnicianId,
            TechnicianName = review.Technician?.FullName ?? "",
            CreatedAt = review.CreatedAt
        };
    }
}
