using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PRN232.LMS.API.Validation;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.RequestModels;

public sealed class ListRequestModel
{
    public string? Search { get; set; }
    public string? Sort { get; set; }
    [Range(1, int.MaxValue)] public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Fields { get; set; }
    public string? Expand { get; set; }
    public QueryOptions ToBusiness() => new(Search, Sort, Page, Size, Fields, Expand);
}
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class StudentRequestModel
{
    [Required]
    [StringLength(100)] public string FullName { get; set; } = "";
    [Required]
    [EmailAddress]
    [StringLength(100)] public string Email { get; set; } = "";
    [Required, PastDate] public DateTime? DateOfBirth { get; set; }
    [Phone]
    [StringLength(30)] public string? PhoneNumber { get; set; }
    public StudentBusinessModel ToBusiness() => new(FullName, Email, DateOfBirth!.Value, PhoneNumber);
}
