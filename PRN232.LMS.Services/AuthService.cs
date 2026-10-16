using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PRN232.LMS.Repositories;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services;

public sealed record JwtSettings(string Key, string Issuer, string Audience);
public sealed record TokenBusinessModel(string AccessToken, string RefreshToken, int ExpiresIn);
public interface IAuthService
{
    Task<TokenBusinessModel> LoginAsync(string username, string password, CancellationToken ct);
    Task<TokenBusinessModel> RefreshAsync(string refreshToken, CancellationToken ct);
}

public sealed class AuthService(IAuthRepository repository, JwtSettings settings) : IAuthService
{
    private readonly PasswordHasher<User> hasher = new();
    public async Task<TokenBusinessModel> LoginAsync(string username, string password, CancellationToken ct)
    {
        var user = await repository.FindUserAsync(username, ct);
        if (user is null || hasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
            throw new ServiceException(401, "Invalid username or password.");
        return await IssueAsync(user, ct);
    }
    public async Task<TokenBusinessModel> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var user = await repository.ConsumeTokenAsync(Hash(refreshToken), DateTime.UtcNow, ct)
            ?? throw new ServiceException(401, "Invalid or expired refresh token.");
        return await IssueAsync(user, ct);
    }
    private async Task<TokenBusinessModel> IssueAsync(User user, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(settings.Issuer, settings.Audience,
            [new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), new("name", user.Username),
             new("role", user.Role), new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())],
            now, now.AddHours(1), new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)), SecurityAlgorithms.HmacSha256));
        var refresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        await repository.AddTokenAsync(new RefreshToken { UserId = user.UserId, TokenHash = Hash(refresh), ExpiresAt = now.AddDays(7) }, ct);
        return new(new JwtSecurityTokenHandler().WriteToken(jwt), refresh, 3600);
    }
    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

// Registered after the database initializer; passwords are hashed before persistence.
public sealed class AuthSeedService(IServiceProvider services) : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        using var scope = services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAuthRepository>();
        var hasher = new PasswordHasher<User>();
        foreach (var (name, password, role) in new[] { ("admin", "Admin@123", "Admin"), ("student", "Student@123", "Student") })
        {
            if (await repository.FindUserAsync(name, ct) is not null) continue;
            var user = new User { Username = name, Role = role };
            user.PasswordHash = hasher.HashPassword(user, password);
            await repository.AddUserAsync(user, ct);
        }
    }
    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
