using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using PRN232.LMS.Repositories;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services;

public interface ILmsService
{
    Task<PageBusinessModel> ListAsync(string resource, QueryOptions options, CancellationToken ct);
    Task<RecordBusinessModel> DetailAsync(string resource, int id, CancellationToken ct);
    Task<RecordBusinessModel> CreateAsync(StudentBusinessModel student, CancellationToken ct);
    Task<RecordBusinessModel> UpdateAsync(int id, StudentBusinessModel student, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task<bool> IsReadyAsync(CancellationToken ct);
}

public sealed class LmsService(ILmsRepository repository) : ILmsService
{
    private record Shape(string[] Scalars, string[] Search, string[] Relations);
    private static readonly Dictionary<string, Shape> Shapes = new()
    {
        ["semesters"] = new(["semesterId", "semesterName", "startDate", "endDate"], ["SemesterName"], ["courses"]),
        ["subjects"] = new(["subjectId", "subjectCode", "subjectName", "credit"], ["SubjectCode", "SubjectName"], []),
        ["courses"] = new(["courseId", "courseName", "semesterId"], ["CourseName"], ["semester", "enrollments"]),
        ["students"] = new(["studentId", "fullName", "email", "dateOfBirth"], ["FullName", "Email"], ["enrollments"]),
        ["enrollments"] = new(["enrollmentId", "studentId", "courseId", "enrollDate", "status"], ["Status"], ["student", "course"])
    };
    private static string Pascal(string value) => char.ToUpperInvariant(value[0]) + value[1..];
    private static string[] Tokens(string? value) => value is null ? [] : value.Split(',').Select(x => x.Trim()).ToArray();

    public Task<PageBusinessModel> ListAsync(string resource, QueryOptions options, CancellationToken ct) => resource switch
    {
        "semesters" => List<Semester>(resource, options, ct), "subjects" => List<Subject>(resource, options, ct),
        "courses" => List<Course>(resource, options, ct), "students" => List<Student>(resource, options, ct),
        "enrollments" => List<Enrollment>(resource, options, ct), _ => throw new ServiceException(404, "Resource not found.")
    };
    private async Task<PageBusinessModel> List<T>(string resource, QueryOptions options, CancellationToken ct) where T : class
    {
        var shape = Shapes[resource];
        if (options.Page < 1 || options.Size < 1) throw new ServiceException(400, "page and size must be positive integers.");
        var size = Math.Min(options.Size, 100);
        var fields = Tokens(options.Fields);
        var expand = Tokens(options.Expand);
        var order = Tokens(options.Sort);
        if (fields.Any(x => !shape.Scalars.Contains(x) && !shape.Relations.Contains(x))) throw new ServiceException(400, "Unknown fields parameter.");
        if (expand.Any(x => !shape.Relations.Contains(x))) throw new ServiceException(400, "Unknown expand parameter.");
        if (order.Any(x => !shape.Scalars.Contains(x.StartsWith('-') ? x[1..] : x))) throw new ServiceException(400, "Unknown sort parameter.");
        var sort = order.Select(x => (Column: Pascal(x.StartsWith('-') ? x[1..] : x), Descending: x.StartsWith('-'))).ToList();
        var primary = Pascal(shape.Scalars[0]);
        if (!sort.Any(x => x.Column == primary)) sort.Add((primary, false));
        var result = await repository.ListAsync<T>(new(options.Search, shape.Search, sort.ToArray(), options.Page, size, expand.Select(Pascal).ToArray()), ct);
        var items = result.Items.Select(entity =>
        {
            var values = Map(entity, expand);
            if (fields.Length > 0)
                values = fields.Distinct().ToDictionary(field => field, field => values.GetValueOrDefault(field));
            return new RecordBusinessModel(values);
        }).ToList();
        return new(items, options.Page, size, result.Total);
    }
    public Task<RecordBusinessModel> DetailAsync(string resource, int id, CancellationToken ct) => resource switch
    {
        "semesters" => Detail<Semester>(resource, id, ct), "subjects" => Detail<Subject>(resource, id, ct),
        "courses" => Detail<Course>(resource, id, ct), "students" => Detail<Student>(resource, id, ct),
        "enrollments" => Detail<Enrollment>(resource, id, ct), _ => throw new ServiceException(404, "Resource not found.")
    };
    private async Task<RecordBusinessModel> Detail<T>(string resource, int id, CancellationToken ct) where T : class
    {
        var relations = Shapes[resource].Relations;
        var entity = await repository.FindAsync<T>(id, relations.Select(Pascal).ToArray(), ct)
            ?? throw new ServiceException(404, "Record not found.");
        return new(Map(entity, relations));
    }
    private static Dictionary<string, object?> Map(object entity, string[] relations)
    {
        // Only scalar properties cross the layer boundary. Relationships are finite projections.
        var values = entity.GetType().GetProperties()
            .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string))
            .ToDictionary(p => JsonNamingPolicy.CamelCase.ConvertName(p.Name), p => p.GetValue(entity));
        foreach (var relation in relations)
        {
            var nested = entity.GetType().GetProperty(Pascal(relation))!.GetValue(entity);
            values[relation] = nested is System.Collections.IEnumerable collection
                ? collection.Cast<object>().Select(x => Map(x, [])).ToArray()
                : nested is null ? null : Map(nested, []);
        }
        return values;
    }
    private static StudentBusinessModel Validate(StudentBusinessModel student)
    {
        var name = student.FullName.Trim(); var email = student.Email.Trim();
        if (name.Length is 0 or >100) throw new ServiceException(400, "fullName must contain 1–100 characters.");
        if (email.Length > 100 || !new EmailAddressAttribute().IsValid(email) || email.Any(c => c > 127))
            throw new ServiceException(400, "email must be a valid address of at most 100 ASCII characters.");
        if (student.DateOfBirth < new DateTime(1753, 1, 1) || student.DateOfBirth.Date > DateTime.UtcNow.Date)
            throw new ServiceException(400, "dateOfBirth must be a valid past or current date.");
        return new(name, email, student.DateOfBirth.Date);
    }
    public async Task<RecordBusinessModel> CreateAsync(StudentBusinessModel student, CancellationToken ct)
    {
        var valid = Validate(student);
        var entity = await repository.AddStudentAsync(new Student { FullName = valid.FullName, Email = valid.Email, DateOfBirth = valid.DateOfBirth }, ct);
        return new(Map(entity, []));
    }
    public async Task<RecordBusinessModel> UpdateAsync(int id, StudentBusinessModel student, CancellationToken ct)
    {
        var valid = Validate(student);
        if (!await repository.UpdateStudentAsync(id, valid.FullName, valid.Email, valid.DateOfBirth, ct)) throw new ServiceException(404, "Student not found.");
        return await DetailAsync("students", id, ct);
    }
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        if (!await repository.DeleteStudentAsync(id, ct)) throw new ServiceException(404, "Student not found.");
    }
    public Task<bool> IsReadyAsync(CancellationToken ct) => repository.IsReadyAsync(ct);
}
