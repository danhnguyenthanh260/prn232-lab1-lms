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
        ["students"] = new("sinh viên", "studentId,fullName,email,dateOfBirth", "fullName, email", "nguyen", "fullName,-dateOfBirth", "studentId,fullName", "enrollments"),
        ["semesters"] = new("học kỳ", "semesterId,semesterName,startDate,endDate", "semesterName", "semester", "semesterName,-startDate", "semesterId,semesterName", "courses"),
        ["subjects"] = new("môn học", "subjectId,subjectCode,subjectName,credit", "subjectCode, subjectName", "subject", "-credit,subjectName", "subjectId,subjectName", ""),
        ["courses"] = new("lớp học", "courseId,courseName,semesterId", "courseName", "course", "semesterId,courseName", "courseId,courseName", "semester,enrollments"),
        ["enrollments"] = new("đăng ký học", "enrollmentId,studentId,courseId,enrollDate,status", "status", "active", "-enrollDate,enrollmentId", "enrollmentId,status", "student,course")
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var segments = context.ApiDescription.RelativePath?.Split('/') ?? [];
        if (segments.Length < 2 || !Guides.TryGetValue(segments[1], out var guide)) return;
        var method = context.ApiDescription.HttpMethod;
        var list = method == "GET" && segments.Length == 2;
        operation.Summary = (method, list) switch
        {
            ("GET", true) => $"Danh sách {guide.Label}",
            ("GET", false) => $"Chi tiết {guide.Label} và dữ liệu liên quan",
            ("POST", _) => "Thêm sinh viên", ("PUT", _) => "Cập nhật sinh viên", _ => "Xóa sinh viên"
        };
        operation.Description = list
            ? "Nhấn **Try it out**, nhập giá trị (không nhập `sort=` hoặc dấu `?`), rồi **Execute**. " +
              "Có thể kết hợp các tham số. Bỏ trống tham số tùy chọn nếu không dùng. Tên trường phân biệt hoa/thường, dùng camelCase. " +
              "Tên sort/fields/expand không hợp lệ trả **400**.\n\n" +
              $"Ví dụ: `/api/{segments[1]}?sort={guide.SortExample}&page=1&size=10&fields={guide.FieldsExample}`. " +
              "Nếu kết hợp fields và expand, fields quyết định tập khóa cuối cùng; phải thêm tên quan hệ vào fields để thấy dữ liệu mở rộng."
            : method == "GET" ? "Nhập ID hiện có (ví dụ 1). ID không tồn tại trả 404. Trả các quan hệ trực tiếp, không lặp vòng."
            : method == "DELETE" ? "Xóa sinh viên và các enrollments của sinh viên đó. Thành công: 200; ID không tồn tại: 404. Thao tác thay đổi dữ liệu thật."
            : "Body JSON phải có đủ fullName, email, dateOfBirth; không gửi studentId. Ngày dùng ISO 8601. " +
              (method == "POST" ? "Thành công: 201 kèm Location." : "Gửi đủ cả 3 trường để cập nhật; thành công: 200; ID không tồn tại: 404.") +
              " Dữ liệu không hợp lệ: 400. Thao tác lưu dữ liệu thật.";

        foreach (var parameter in operation.Parameters)
        {
            parameter.Description = parameter.Name switch
            {
                "search" => $"Tìm chuỗi chứa, không phân biệt hoa/thường, trên: `{guide.SearchFields}`. Bỏ trống để không lọc. Ví dụ: `{guide.SearchExample}`.",
                "sort" => $"Các trường hợp lệ: `{guide.Scalars}`. Không có dấu = tăng dần; tiền tố `-` = giảm dần. Phân cách nhiều trường bằng dấu phẩy, ưu tiên từ trái sang phải. Ví dụ: `{guide.SortExample}`. Bỏ trống: ID tăng dần; ID cũng dùng để ổn định thứ tự khi bằng nhau.",
                "page" => "Trang bắt đầu từ 1. Mặc định 1; nhỏ hơn 1 hoặc sai kiểu trả 400. Trang vượt dữ liệu trả danh sách rỗng.",
                "size" => "Số dòng/trang, mặc định 10. Từ 1 đến 100; lớn hơn 100 tự giới hạn về 100; nhỏ hơn 1 hoặc sai kiểu trả 400. pagination.pageSize là kích thước thực tế.",
                "fields" => $"Chọn đúng các khóa muốn nhận, cách nhau bằng dấu phẩy. Trường: `{guide.Scalars}`. " +
                    (guide.Relations.Length == 0 ? "Không có trường quan hệ. " : $"Quan hệ có thể chọn: `{guide.Relations}` (cần expand tương ứng, nếu không giá trị là null). ") +
                    $"Ví dụ: `{guide.FieldsExample}`. Bỏ trống: trả đủ trường scalar và các quan hệ đã expand. Không làm mất pagination ở root.",
                "expand" => guide.Relations.Length == 0
                    ? "Subject không có quan hệ trong schema đề bài: để trống, không gửi expand. Mọi giá trị không rỗng đều trả 400."
                    : $"Quan hệ hợp lệ: `{guide.Relations}`. Phân cách bằng dấu phẩy. Ví dụ: `{guide.Relations}`. Bỏ trống: không trả quan hệ. Khi dùng fields, phải chọn cả tên quan hệ trong fields.",
                "id" => "ID nguyên của bản ghi, ví dụ 1. Không tồn tại trả 404.",
                _ => parameter.Description
            };
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
        schema.Description = "Dữ liệu tạo/cập nhật sinh viên. Cả 3 trường bắt buộc; ID do server cấp.";
        schema.Required.UnionWith(["fullName", "email", "dateOfBirth"]);
        schema.Properties["fullName"].Description = "Họ tên sau khi bỏ khoảng trắng đầu/cuối: 1–100 ký tự, không được chỉ gồm khoảng trắng.";
        schema.Properties["email"].Description = "Email hợp lệ, tối đa 100 ký tự ASCII. Không bắt buộc duy nhất.";
        schema.Properties["dateOfBirth"].Description = "Ngày sinh ISO 8601, từ 1753-01-01 đến hôm nay; server chỉ lưu phần ngày. Ví dụ: 2003-01-15.";
        schema.Example = new OpenApiObject
        {
            ["fullName"] = new OpenApiString("Nguyen Van An"),
            ["email"] = new OpenApiString("an@example.edu"),
            ["dateOfBirth"] = new OpenApiString("2003-01-15")
        };
    }
}
