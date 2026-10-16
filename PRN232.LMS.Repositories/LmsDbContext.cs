using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories;

public sealed class LmsDbContext(DbContextOptions<LmsDbContext> options) : DbContext(options)
{
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().ToTable("Users");
        b.Entity<User>().HasIndex(x => x.Username).IsUnique();
        b.Entity<User>().Property(x => x.Username).HasColumnType("varchar(50)").IsRequired();
        b.Entity<User>().Property(x => x.PasswordHash).HasColumnType("varchar(255)").IsRequired();
        b.Entity<User>().Property(x => x.Role).HasColumnType("varchar(20)").IsRequired();
        b.Entity<RefreshToken>().HasIndex(x => x.TokenHash).IsUnique();
        b.Entity<RefreshToken>().Property(x => x.TokenHash).HasColumnType("varchar(64)").IsRequired();
        b.Entity<RefreshToken>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        b.Entity<Student>().Property(x => x.PhoneNumber).HasColumnType("varchar(30)");
        b.Entity<Semester>().Property(x => x.SemesterName).HasColumnType("nvarchar(100)").IsRequired();
        b.Entity<Semester>().Property(x => x.StartDate).HasColumnType("datetime");
        b.Entity<Semester>().Property(x => x.EndDate).HasColumnType("datetime");
        b.Entity<Subject>().Property(x => x.SubjectCode).HasColumnType("varchar(20)").IsRequired();
        b.Entity<Subject>().Property(x => x.SubjectName).HasColumnType("nvarchar(100)").IsRequired();
        b.Entity<Course>().Property(x => x.CourseName).HasColumnType("nvarchar(100)").IsRequired();
        b.Entity<Student>().Property(x => x.FullName).HasColumnType("nvarchar(100)").IsRequired();
        b.Entity<Student>().Property(x => x.Email).HasColumnType("varchar(100)").IsRequired();
        b.Entity<Student>().Property(x => x.DateOfBirth).HasColumnType("datetime");
        b.Entity<Enrollment>().Property(x => x.Status).HasColumnType("varchar(20)").IsRequired();
        b.Entity<Enrollment>().Property(x => x.EnrollDate).HasColumnType("datetime");
        b.Entity<Course>().HasOne(x => x.Semester).WithMany(x => x.Courses).HasForeignKey(x => x.SemesterId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Enrollment>().HasOne(x => x.Course).WithMany(x => x.Enrollments).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Enrollment>().HasOne(x => x.Student).WithMany(x => x.Enrollments).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
    }
}
