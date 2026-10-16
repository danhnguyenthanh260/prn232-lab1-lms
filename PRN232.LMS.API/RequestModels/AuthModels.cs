using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.RequestModels;

public sealed class LoginRequestModel
{
    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[A-Za-z0-9_.-]+$")]
    public string Username { get; set; } = "";
    [Required]
    [StringLength(200)]
    public string Password { get; set; } = "";
}
public sealed class RefreshRequestModel
{
    [Required]
    [StringLength(200)]
    public string RefreshToken { get; set; } = "";
}
