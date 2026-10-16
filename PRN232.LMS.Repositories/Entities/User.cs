namespace PRN232.LMS.Repositories.Entities;

public sealed class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "";
}

public sealed class RefreshToken
{
    public int RefreshTokenId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string TokenHash { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
