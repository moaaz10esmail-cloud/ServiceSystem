using Microsoft.AspNetCore.Mvc;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Enums;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Shared.Common;
using ServicesSystem.Shared.DTOs.Payments;

namespace ServicesSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<ActionResult<Result<PaymentDto>>> CreatePayment([FromBody] CreatePaymentDto dto)
    {
        // Validate request exists
        var request = await _unitOfWork.Requests.GetByIdAsync(dto.RequestId);
        if (request == null)
        {
            return NotFound(Result<PaymentDto>.Failure("Request not found"));
        }

        // Check if payment already exists for this request
        var existingPayment = await _unitOfWork.Payments.GetByRequestIdAsync(dto.RequestId);
        if (existingPayment != null)
        {
            return BadRequest(Result<PaymentDto>.Failure("Payment already exists for this request"));
        }

        var payment = new Payment
        {
            RequestId = dto.RequestId,
            CustomerId = request.CustomerId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            PaymentMethod = dto.PaymentMethod,
            PaymentGateway = dto.PaymentGateway,
            Status = PaymentStatus.Pending
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        var paymentDto = MapToDto(payment);
        return CreatedAtAction(
            nameof(GetById),
            new { id = payment.Id },
            Result<PaymentDto>.Success(paymentDto, "Payment created successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<PaymentDto>>> GetById(Guid id)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id);
        if (payment == null)
        {
            return NotFound(Result<PaymentDto>.Failure("Payment not found"));
        }

        return Ok(Result<PaymentDto>.Success(MapToDto(payment)));
    }

    [HttpGet("request/{requestId}")]
    public async Task<ActionResult<Result<PaymentDto>>> GetByRequestId(Guid requestId)
    {
        var payment = await _unitOfWork.Payments.GetByRequestIdAsync(requestId);
        if (payment == null)
        {
            return NotFound(Result<PaymentDto>.Failure("Payment not found for this request"));
        }

        return Ok(Result<PaymentDto>.Success(MapToDto(payment)));
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<Result<IEnumerable<PaymentDto>>>> GetByCustomerId(Guid customerId)
    {
        var payments = await _unitOfWork.Payments.GetByCustomerIdAsync(customerId);
        var paymentDtos = payments.Select(MapToDto);

        return Ok(Result<IEnumerable<PaymentDto>>.Success(paymentDtos));
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<Result>> UpdateStatus(Guid id, [FromBody] UpdatePaymentStatusDto dto)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id);
        if (payment == null)
        {
            return NotFound(Result.Failure("Payment not found"));
        }

        if (!Enum.TryParse<PaymentStatus>(dto.Status, out var status))
        {
            return BadRequest(Result.Failure("Invalid payment status"));
        }

        payment.Status = status;
        payment.TransactionId = dto.TransactionId;
        payment.FailureReason = dto.FailureReason;

        if (status == PaymentStatus.Completed && !payment.PaidAt.HasValue)
        {
            payment.PaidAt = DateTime.UtcNow;
        }

        await _unitOfWork.Payments.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return Ok(Result.Success("Payment status updated successfully"));
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status.ToString(),
            PaymentMethod = payment.PaymentMethod,
            PaymentGateway = payment.PaymentGateway,
            TransactionId = payment.TransactionId,
            FailureReason = payment.FailureReason,
            PaidAt = payment.PaidAt,
            CreatedAt = payment.CreatedAt,
            RequestId = payment.RequestId,
            CustomerId = payment.CustomerId,
            CustomerName = payment.Customer?.FullName ?? ""
        };
    }
}
