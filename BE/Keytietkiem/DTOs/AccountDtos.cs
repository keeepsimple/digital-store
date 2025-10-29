using System.ComponentModel.DataAnnotations;

namespace Keytietkiem.DTOs;

/// <summary>
/// Request DTO for sending OTP to email
/// </summary>
public class SendOtpDto
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for verifying OTP
/// </summary>
public class VerifyOtpDto
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP là bắt buộc")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP phải có 6 ký tự")]
    public string Otp { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO after OTP verification
/// </summary>
public class OtpVerificationResponseDto
{
    public bool IsVerified { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? VerificationToken { get; set; }
}

/// <summary>
/// Request DTO for user registration with OTP verification
/// </summary>
public class RegisterDto
{
    [Required(ErrorMessage = "Username là bắt buộc")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "Username phải từ 3-60 ký tự")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải từ 6-100 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "FirstName là bắt buộc")]
    [StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastName là bắt buộc")]
    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(32)]
    public string? Phone { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [Required(ErrorMessage = "VerificationToken là bắt buộc")]
    public string VerificationToken { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for user login
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "Username là bắt buộc")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password là bắt buộc")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO returned after successful login or registration
/// </summary>
public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfoDto User { get; set; } = null!;
}

/// <summary>
/// User information included in login response
/// </summary>
public class UserInfoDto
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

/// <summary>
/// Request DTO for refreshing access token
/// </summary>
public class RefreshTokenDto
{
    [Required(ErrorMessage = "RefreshToken là bắt buộc")]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for changing password
/// </summary>
public class ChangePasswordDto
{
    [Required(ErrorMessage = "CurrentPassword là bắt buộc")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "NewPassword là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "NewPassword phải từ 6-100 ký tự")]
    public string NewPassword { get; set; } = string.Empty;
}
