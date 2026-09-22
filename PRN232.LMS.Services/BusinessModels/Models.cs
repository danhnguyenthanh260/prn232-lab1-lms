namespace PRN232.LMS.Services.BusinessModels;

public record QueryOptions(string? Search = null, string? Sort = null, int Page = 1, int Size = 10, string? Fields = null, string? Expand = null);
public record RecordBusinessModel(IReadOnlyDictionary<string, object?> Values);
public record PageBusinessModel(IReadOnlyList<RecordBusinessModel> Items, int Page, int PageSize, int TotalItems);
public record StudentBusinessModel(string FullName, string Email, DateTime DateOfBirth);
public sealed class ServiceException(int status, string message) : Exception(message)
{
    public int Status { get; } = status;
}
