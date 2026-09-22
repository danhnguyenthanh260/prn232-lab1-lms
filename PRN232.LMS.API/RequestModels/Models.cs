using System.ComponentModel.DataAnnotations;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.RequestModels;

public sealed class ListRequestModel
{
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Fields { get; set; }
    public string? Expand { get; set; }
    public QueryOptions ToBusiness() => new(Search, Sort, Page, Size, Fields, Expand);
}
public sealed class StudentRequestModel
{
    [Required, StringLength(100)] public string FullName { get; set; } = "";
    [Required, EmailAddress, StringLength(100)] public string Email { get; set; } = "";
    [Required] public DateTime? DateOfBirth { get; set; }
    public StudentBusinessModel ToBusiness() => new(FullName, Email, DateOfBirth!.Value);
}
