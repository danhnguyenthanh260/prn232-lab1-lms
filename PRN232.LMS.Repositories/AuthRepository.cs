using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories;

public interface IAuthRepository
{
    Task<User?> FindUserAsync(string username, CancellationToken ct);
    Task AddUserAsync(User user, CancellationToken ct);
    Task AddTokenAsync(RefreshToken token, CancellationToken ct);
    Task<User?> ConsumeTokenAsync(string hash, DateTime now, CancellationToken ct);
}

public sealed class AuthRepository(LmsDbContext db) : IAuthRepository
{
    public Task<User?> FindUserAsync(string username, CancellationToken ct) =>
        db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Username == username, ct);
    public async Task AddUserAsync(User user, CancellationToken ct)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
    }
    public async Task AddTokenAsync(RefreshToken token, CancellationToken ct)
    {
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync(ct);
    }
    public async Task<User?> ConsumeTokenAsync(string hash, DateTime now, CancellationToken ct)
    {
        // Atomic update prevents two concurrent requests from exchanging the same token.
        var changed = await db.RefreshTokens.Where(x => x.TokenHash == hash && x.RevokedAt == null && x.ExpiresAt > now)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAt, now), ct);
        return changed == 1
            ? await db.RefreshTokens.Where(x => x.TokenHash == hash).Select(x => x.User).AsNoTracking().SingleAsync(ct)
            : null;
    }
}
