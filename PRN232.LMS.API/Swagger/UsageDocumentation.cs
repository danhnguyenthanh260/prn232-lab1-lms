using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PRN232.LMS.API.RequestModels;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PRN232.LMS.API.Swagger;

public sealed class UsageDocumentation : IOperationFilter
{
    private record Guide(string Label, string Scalars, string SearchFields, string SearchExample,
        string SortExample, string FieldsExample, string Relations);
    private static readonly Dictionary<string, Guide> Guides = new()
    {
        ["students"] = new("sinh viên", "studentId,fullName,email,dateOfBirth,phoneNumber", "fullName, email", "nguyen", "fullName,-dateOfBirth", "studentId,fullName", "enrollments"),
        ["semesters"] = new("học kỳ", "semesterId,semesterName,startDate,endDate", "semesterName", "semester", "semesterName,-startDate", "semesterId,semesterName", "courses"),
        ["subjects"] = new("môn học", "subjectId,subjectCode,subjectName,credit", "subjectCode, subjectName", "subject", "-credit,subjectName", "subjectId,subjectName", ""),
        ["courses"] = new("lớp học", "courseId,courseName,semesterId", "courseName", "course", "semesterId,courseName", "courseId,courseName", "semester,enrollments"),
        ["enrollments"] = new("đăng ký học", "enrollmentId,studentId,courseId,enrollDate,status", "status", "active", "-enrollDate,enrollmentId", "enrollmentId,status", "student,course")
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var segments = context.ApiDescription.RelativePath?.Split('/') ?? [];
        var resourceIndex = Array.FindIndex(segments, segment => Guides.ContainsKey(segment));
        if (resourceIndex < 0 || !Guides.TryGetValue(segments[resourceIndex], out var guide)) return;
        var method = context.ApiDescription.HttpMethod;
        var list = method == "GET" && segments.Length == resourceIndex + 1;
        operation.Summary = (method, list) switch
        {
            ("GET", true) => $"Danh sách {guide.Label}",
            ("GET", false) => $"Chi tiết {guide.Label} và dữ liệu liên quan",
            ("POST", _) => "Thêm sinh viên", ("PUT", _) => "Cập nhật sinh viên", _ => "Xóa sinh viên"
        };
        foreach (var parameter in operation.Parameters)
        {
            parameter.Example = parameter.Name switch
            {
                "search" => new OpenApiString(guide.SearchExample), "sort" => new OpenApiString(guide.SortExample),
                "fields" => new OpenApiString(guide.FieldsExample),
                "expand" when guide.Relations.Length > 0 => new OpenApiString(guide.Relations),
                "id" or "page" => new OpenApiInteger(1), "size" => new OpenApiInteger(10), _ => null
            };
            if (parameter.Name is "page" or "size")
            {
                parameter.Schema.Minimum = 1;
                parameter.Schema.Default = new OpenApiInteger(parameter.Name == "page" ? 1 : 10);
            }
        }
    }
}

public sealed class StudentDocumentation : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(StudentRequestModel)) return;
        schema.Required.UnionWith(["fullName", "email", "dateOfBirth"]);
        schema.Example = new OpenApiObject
        {
            ["fullName"] = new OpenApiString("Nguyen Van An"),
            ["email"] = new OpenApiString("an@example.edu"),
            ["dateOfBirth"] = new OpenApiString("2003-01-15")
        };
    }
}
