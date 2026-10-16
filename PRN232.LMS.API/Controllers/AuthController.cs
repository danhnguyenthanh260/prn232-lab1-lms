using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.RequestModels;
using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.Services;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService service;
    public AuthController(IAuthService service) => this.service = service;

    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel request, CancellationToken ct) =>
        Ok(new ApiResponseModel(true, "Login successful", await service.LoginAsync(request.Username, request.Password, ct), null));

    [AllowAnonymous, HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestModel request, CancellationToken ct) =>
        Ok(new ApiResponseModel(true, "Token refreshed", await service.RefreshAsync(request.RefreshToken, ct), null));

    [Authorize, HttpGet("me")]
    public IActionResult Me() => Ok(new ApiResponseModel(true, "Success", new { username = User.Identity?.Name, role = User.FindFirst("role")?.Value }, null));
}
