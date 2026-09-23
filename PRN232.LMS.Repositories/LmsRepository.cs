using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories;

public record QuerySpec(string? Search, string[] SearchColumns, (string Column, bool Descending)[] Order,
    int Page, int Size, string[] Includes);
public record EntityPage<T>(List<T> Items, int Total);

public interface ILmsRepository
{
    Task<EntityPage<T>> ListAsync<T>(QuerySpec spec, CancellationToken ct) where T : class;
    Task<T?> FindAsync<T>(int id, string[] includes, CancellationToken ct) where T : class;
    Task<Student> AddStudentAsync(Student student, CancellationToken ct);
    Task<bool> UpdateStudentAsync(int id, string name, string email, DateTime birth, CancellationToken ct);
    Task<bool> DeleteStudentAsync(int id, CancellationToken ct);
    Task<bool> IsReadyAsync(CancellationToken ct);
}

public sealed class LmsRepository(LmsDbContext db) : ILmsRepository
{
    public async Task<EntityPage<T>> ListAsync<T>(QuerySpec spec, CancellationToken ct) where T : class
    {
        IQueryable<T> query = db.Set<T>().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(spec.Search))
        {
            var item = Expression.Parameter(typeof(T), "item");
            Expression? combined = null;
            foreach (var field in spec.SearchColumns)
            {
                var lowered = Expression.Call(Expression.Property(item, field), nameof(string.ToLower), Type.EmptyTypes);
                var contains = Expression.Call(lowered, nameof(string.Contains), Type.EmptyTypes, Expression.Constant(spec.Search.ToLowerInvariant()));
                combined = combined is null ? contains : Expression.OrElse(combined, contains);
            }
            query = query.Where(Expression.Lambda<Func<T, bool>>(combined!, item));
        }
        var count = await query.CountAsync(ct);
        var first = true;
        foreach (var (column, descending) in spec.Order)
        {
            var parameter = Expression.Parameter(typeof(T), "item");
            Expression property = Expression.Property(parameter, column);
            if (property.Type == typeof(string))
                property = Expression.Call(typeof(RelationalDbFunctionsExtensions), nameof(RelationalDbFunctionsExtensions.Collate), [typeof(string)],
                    Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                    Expression.Call(property, nameof(string.ToUpper), Type.EmptyTypes), Expression.Constant("Latin1_General_100_BIN2"));
            var method = first ? (descending ? "OrderByDescending" : "OrderBy") : (descending ? "ThenByDescending" : "ThenBy");
            query = query.Provider.CreateQuery<T>(Expression.Call(typeof(Queryable), method,
                [typeof(T), property.Type], query.Expression, Expression.Quote(Expression.Lambda(property, parameter))));
            first = false;
        }
        if ((long)(spec.Page - 1) * spec.Size >= count) return new([], count);
        query = query.Skip((spec.Page - 1) * spec.Size).Take(spec.Size);
        foreach (var relation in spec.Includes) query = query.Include(relation);
        return new(await query.AsSplitQuery().ToListAsync(ct), count);
    }

    public async Task<T?> FindAsync<T>(int id, string[] includes, CancellationToken ct) where T : class
    {
        IQueryable<T> query = db.Set<T>().AsNoTracking();
        foreach (var relation in includes) query = query.Include(relation);
        return await query.AsSplitQuery().FirstOrDefaultAsync(x => EF.Property<int>(x, typeof(T).Name + "Id") == id, ct);
    }
    public async Task<Student> AddStudentAsync(Student student, CancellationToken ct)
    {
        db.Students.Add(student);
        await db.SaveChangesAsync(ct);
        return student;
    }
    public async Task<bool> UpdateStudentAsync(int id, string name, string email, DateTime birth, CancellationToken ct)
    {
        var student = await db.Students.FindAsync([id], ct);
        if (student is null) return false;
        student.FullName = name; student.Email = email; student.DateOfBirth = birth;
        await db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> DeleteStudentAsync(int id, CancellationToken ct)
    {
        var student = await db.Students.FindAsync([id], ct);
        if (student is null) return false;
        db.Students.Remove(student);
        await db.SaveChangesAsync(ct);
        return true;
    }
    public Task<bool> IsReadyAsync(CancellationToken ct) => db.Database.CanConnectAsync(ct);
}
