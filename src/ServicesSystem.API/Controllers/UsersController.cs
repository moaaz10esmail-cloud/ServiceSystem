using Microsoft.AspNetCore.Mvc;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Shared.Common;
using ServicesSystem.Shared.DTOs.Users;

namespace ServicesSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<UserDto>>>> GetAll()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role.ToString(),
            AvatarUrl = u.AvatarUrl,
            IsActive = u.IsActive,
            Rating = u.Rating,
            TotalReviews = u.TotalReviews,
            CreatedAt = u.CreatedAt
        });

        return Ok(Result<IEnumerable<UserDto>>.Success(userDtos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<UserDto>>> GetById(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(Result<UserDto>.Failure("User not found"));
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            Rating = user.Rating,
            TotalReviews = user.TotalReviews,
            CreatedAt = user.CreatedAt
        };

        return Ok(Result<UserDto>.Success(userDto));
    }

    [HttpPost]
    public async Task<ActionResult<Result<UserDto>>> Create([FromBody] CreateUserDto dto)
    {
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return BadRequest(Result<UserDto>.Failure("User with this email already exists"));
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Role = Enum.Parse<Domain.Enums.UserRole>(dto.Role),
            AvatarUrl = dto.AvatarUrl
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            Rating = user.Rating,
            TotalReviews = user.TotalReviews,
            CreatedAt = user.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, Result<UserDto>.Success(userDto, "User created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(Result.Failure("User not found"));
        }

        if (!string.IsNullOrEmpty(dto.FirstName))
            user.FirstName = dto.FirstName;
        if (!string.IsNullOrEmpty(dto.LastName))
            user.LastName = dto.LastName;
        if (!string.IsNullOrEmpty(dto.PhoneNumber))
            user.PhoneNumber = dto.PhoneNumber;
        if (dto.AvatarUrl != null)
            user.AvatarUrl = dto.AvatarUrl;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Ok(Result.Success("User updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> Delete(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(Result.Failure("User not found"));
        }

        await _unitOfWork.Users.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return Ok(Result.Success("User deleted successfully"));
    }
}
