namespace PRN232.LMS.Repositories.Entities;

public class Semester
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<Course> Courses { get; set; } = [];
}
public class Subject
{
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public int Credit { get; set; }
}
public class Course
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = "";
    public int SemesterId { get; set; }
    public Semester Semester { get; set; } = null!;
    public List<Enrollment> Enrollments { get; set; } = [];
}
public class Student
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public List<Enrollment> Enrollments { get; set; } = [];
}
public class Enrollment
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = "";
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
