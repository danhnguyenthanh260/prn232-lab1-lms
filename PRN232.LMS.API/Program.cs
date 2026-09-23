using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.Repositories;
using PRN232.LMS.Services;
using PRN232.LMS.Services.BusinessModels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRepositories(builder.Configuration.GetConnectionString("Lms")
    ?? throw new InvalidOperationException("Set ConnectionStrings__Lms to a SQL Server connection string."));
builder.Services.AddScoped<ILmsService, LmsService>();
builder.Services.AddControllers(options => options.Conventions.Add(new PRN232.LMS.API.Routing.LowercaseControllerConvention()))
    .ConfigureApiBehaviorOptions(options =>
    options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(ApiResponseModel.Error("Invalid input.",
        context.ModelState.SelectMany(x => x.Value!.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? $"Invalid {x.Key}." : e.ErrorMessage)).ToArray())));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DescribeAllParametersInCamelCase();
    options.OperationFilter<PRN232.LMS.API.Swagger.UsageDocumentation>();
    options.SchemaFilter<PRN232.LMS.API.Swagger.StudentDocumentation>();
});
var app = builder.Build();
app.Use(async (context, next) =>
{
    try { await next(); }
    catch (ServiceException exception)
    {
        context.Response.StatusCode = exception.Status;
        await context.Response.WriteAsJsonAsync(ApiResponseModel.Error(exception.Message));
    }
    catch (Exception exception) when (!context.RequestAborted.IsCancellationRequested)
    {
        app.Logger.LogError(exception, "Request failed.");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(ApiResponseModel.Error("Unexpected server error."));
    }
});
app.UseStatusCodePages(async context => await context.HttpContext.Response.WriteAsJsonAsync(
    ApiResponseModel.Error($"HTTP {context.HttpContext.Response.StatusCode}")));
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/health", async (ILmsService service, CancellationToken ct) =>
    await service.IsReadyAsync(ct) ? Results.Ok(new ApiResponseModel(true, "Ready", new { status = "healthy" }, null))
        : Results.Json(ApiResponseModel.Error("Database unavailable."), statusCode: 503));
app.MapFallback(async context =>
{
    var first = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
    if (first is "students" or "semesters" or "subjects" or "courses" or "enrollments")
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
    }
    else { context.Response.StatusCode = 404; await context.Response.WriteAsJsonAsync(ApiResponseModel.Error("Route not found.")); }
});
await app.RunAsync();
