using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories;

public static class DatabaseSetup
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, string connection)
    {
        services.AddDbContext<LmsDbContext>(options => options.UseSqlServer(connection));
        services.AddScoped<ILmsRepository, LmsRepository>();
        return services;
    }
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        // Each failed connection attempt gets a fresh scope/context.
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
                await db.Database.EnsureCreatedAsync();
                if (await db.Semesters.AnyAsync()) return;
                await using var transaction = await db.Database.BeginTransactionAsync();
                var semesters = Enumerable.Range(1, 5).Select(i => new Semester { SemesterName = $"Semester {i}", StartDate = new(2024 + i / 3, (i % 3) * 4 + 1, 1), EndDate = new(2024 + i / 3, (i % 3) * 4 + 4, 28) }).ToList();
                var subjects = Enumerable.Range(1, 10).Select(i => new Subject { SubjectCode = $"SUB{i:000}", SubjectName = $"Subject {i:00}", Credit = i % 3 + 2 }).ToList();
                var courses = Enumerable.Range(1, 20).Select(i => new Course { CourseName = $"Course {i:00}", Semester = semesters[(i - 1) % 5] }).ToList();
                string[] names = ["Nguyễn Minh Anh", "Trần Hoàng Nam", "Lê Ngọc Linh", "Phạm Gia Huy", "Võ Thanh Hà", "Đặng Bảo An", "Bùi Khánh Vy", "Đỗ Đức Minh", "Hồ Phương Thảo", "Dương Tuấn Kiệt"];
                var students = Enumerable.Range(1, 50).Select(i => new Student { FullName = names[(i - 1) % names.Length] + $" {i:00}", Email = $"student{i:00}@example.edu", DateOfBirth = new(2002 + i % 5, i % 12 + 1, i % 27 + 1) }).ToList();
                var enrollments = Enumerable.Range(0, 500).Select(i => new Enrollment { Student = students[i % 50], Course = courses[(i / 50 + i % 50) % 20], EnrollDate = new DateTime(2025, 1, 1).AddDays(i % 120), Status = i % 4 == 0 ? "completed" : "active" }).ToList();
                db.AddRange(semesters); db.AddRange(subjects); db.AddRange(courses); db.AddRange(students); db.AddRange(enrollments);
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
                return;
            }
            catch (Microsoft.Data.SqlClient.SqlException) when (attempt < 10)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }
    }
}
