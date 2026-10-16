using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (ServiceException exception) when (!context.Response.HasStarted)
        {
            context.Response.StatusCode = exception.Status;
            await context.Response.WriteAsJsonAsync(ApiResponseModel.Error(exception.Message));
        }
        catch (Exception exception) when (!context.RequestAborted.IsCancellationRequested && !context.Response.HasStarted)
        {
            logger.LogError(exception, "Request failed.");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(ApiResponseModel.Error("Unexpected server error."));
        }
    }
}
