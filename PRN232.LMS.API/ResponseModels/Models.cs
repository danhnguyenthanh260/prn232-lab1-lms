using System.Text.Json.Serialization;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.ResponseModels;

public record PaginationResponseModel(int Page, int PageSize, int TotalItems, int TotalPages);
public sealed class ResourceResponseModel : Dictionary<string, object?>
{
    public ResourceResponseModel(RecordBusinessModel model) : base(model.Values) { }
}
public record ApiResponseModel(bool Success, string Message, object? Data, string[]? Errors,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] PaginationResponseModel? Pagination = null)
{
    public static ApiResponseModel Ok(RecordBusinessModel record) => new(true, "Success", new ResourceResponseModel(record), null);
    public static ApiResponseModel List(PageBusinessModel page) => new(true, "Success", page.Items.Select(x => new ResourceResponseModel(x)).ToArray(), null,
        new(page.Page, page.PageSize, page.TotalItems, (int)Math.Ceiling((double)page.TotalItems / page.PageSize)));
    public static ApiResponseModel Error(string message, string[]? errors = null) => new(false, message, null, errors ?? [message]);
}
