using Microsoft.Extensions.Configuration;
using ServicesSystem.Application.Interfaces;
using ServicesSystem.Domain.Entities;
using ServicesSystem.Domain.Enums;
using ServicesSystem.Domain.Interfaces;
using ServicesSystem.Shared.Common;
using ServicesSystem.Shared.DTOs.Auth;

namespace ServicesSystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        // Validate passwords match
        if (dto.Password != dto.ConfirmPassword)
        {
            return Result<AuthResponseDto>.Failure("Passwords do not match");
        }

        // Check if user already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingUser != null)
        {
            return Result<AuthResponseDto>.Failure("User with this email already exists");
        }

        // Validate password strength (basic validation)
        if (dto.Password.Length < 6)
        {
            return Result<AuthResponseDto>.Failure("Password must be at least 6 characters long");
        }

        // Parse role
        if (!Enum.TryParse<UserRole>(dto.Role, out var userRole))
        {
            return Result<AuthResponseDto>.Failure("Invalid role specified");
        }

        // Create user
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Role = userRole,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            PasswordSalt = Guid.NewGuid().ToString(), // Additional salt for extra security
            LastPasswordChangeDate = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenExpirationDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "15")),
            User = new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString(),
                AvatarUrl = user.AvatarUrl
            }
        };

        return Result<AuthResponseDto>.Success(response, "User registered successfully");
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email, cancellationToken);
        if (user == null)
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return Result<AuthResponseDto>.Failure("User account is inactive");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password");
        }

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenExpirationDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "15")),
            User = new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString(),
                AvatarUrl = user.AvatarUrl
            }
        };

        return Result<AuthResponseDto>.Success(response, "Login successful");
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        // Find refresh token
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token, cancellationToken);
        if (refreshToken == null)
        {
            return Result<AuthResponseDto>.Failure("Invalid refresh token");
        }

        // Check if token is active
        if (!refreshToken.IsActive)
        {
            return Result<AuthResponseDto>.Failure("Refresh token is expired or revoked");
        }

        // Get user
        var user = await _unitOfWork.Users.GetByIdAsync(refreshToken.UserId, cancellationToken);
        if (user == null || !user.IsActive)
        {
            return Result<AuthResponseDto>.Failure("User not found or inactive");
        }

        // Generate new access token
        var accessToken = _jwtService.GenerateAccessToken(user);

        var response = new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = token, // Return the same refresh token
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "15")),
            User = new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString(),
                AvatarUrl = user.AvatarUrl
            }
        };

        return Result<AuthResponseDto>.Success(response, "Token refreshed successfully");
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        // Validate new passwords match
        if (dto.NewPassword != dto.ConfirmNewPassword)
        {
            return Result.Failure("New passwords do not match");
        }

        // Validate password strength
        if (dto.NewPassword.Length < 6)
        {
            return Result.Failure("Password must be at least 6 characters long");
        }

        // Get user
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        // Verify current password
        if (!_passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
        {
            return Result.Failure("Current password is incorrect");
        }

        // Check if new password is same as old password
        if (_passwordHasher.VerifyPassword(dto.NewPassword, user.PasswordHash))
        {
            return Result.Failure("New password must be different from current password");
        }

        // Update password
        user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
        user.PasswordSalt = Guid.NewGuid().ToString();
        user.LastPasswordChangeDate = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Revoke all existing refresh tokens for security
        await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Password changed successfully. Please login again.");
    }

    public async Task<Result> RevokeTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token, cancellationToken);
        if (refreshToken == null)
        {
            return Result.Failure("Invalid refresh token");
        }

        if (refreshToken.IsRevoked)
        {
            return Result.Failure("Token already revoked");
        }

        refreshToken.RevokedAt = DateTime.UtcNow;
        await _unitOfWork.RefreshTokens.UpdateAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Token revoked successfully");
    }
}
