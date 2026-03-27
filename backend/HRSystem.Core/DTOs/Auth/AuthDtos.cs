using System.ComponentModel.DataAnnotations;

namespace HRSystem.Core.DTOs.Auth;

public record LoginDto(
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password
);

public record RegisterDto(
    [Required][MaxLength(100)] string FirstName,
    [Required][MaxLength(100)] string LastName,
    [Required][EmailAddress] string Email,
    [Required][MinLength(8)] string Password,
    [Required] string Role
);

public record AuthResponseDto(
    string Token,
    string RefreshToken,
    DateTime TokenExpiry,
    UserInfoDto User
);

public record UserInfoDto(
    int Id,
    string FullName,
    string Email,
    string Role
);

public record RefreshTokenDto(
    [Required] string RefreshToken
);
