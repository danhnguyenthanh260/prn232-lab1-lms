using System.Text;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PRN232.LMS.API.Middleware;
using PRN232.LMS.API.RequestModels;
using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.API.Validation;
using PRN232.LMS.Repositories;
using PRN232.LMS.Services;

var builder = WebApplication.CreateBuilder(args);
// One completion log per request; suppress framework start logs and SQL chatter.
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
builder.Services.AddRepositories(builder.Configuration.GetConnectionString("Lms")
    ?? throw new InvalidOperationException("Set ConnectionStrings__Lms to a SQL Server connection string."));
builder.Services.AddScoped<ILmsService, LmsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<AuthSeedService>();
var key = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Set Jwt__Key in the environment.");
if (Encoding.UTF8.GetByteCount(key) < 32) throw new InvalidOperationException("Jwt__Key must contain at least 32 bytes.");
var jwt = new JwtSettings(key, "PRN232.LMS", "PRN232.LMS.Client");
builder.Services.AddSingleton(jwt);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidIssuer = jwt.Issuer,
        ValidateAudience = true, ValidAudience = jwt.Audience,
        ValidateLifetime = true, ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
        ClockSkew = TimeSpan.Zero, RoleClaimType = "role", NameClaimType = "name",
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
    };
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<IValidator<StudentRequestModel>, StudentRequestValidator>();
builder.Services.AddScoped<StudentValidationFilter>();
builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new PRN232.LMS.API.Routing.LowercaseControllerConvention());
        options.ReturnHttpNotAcceptable = true;
        options.OutputFormatters.Add(new EnvelopeXmlFormatter());
        options.Filters.Add<StudentValidationFilter>();
    })
    .AddXmlSerializerFormatters()
    .ConfigureApiBehaviorOptions(options =>
    options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(ApiResponseModel.Error("Invalid input.",
        context.ModelState.SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {(string.IsNullOrEmpty(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage)}")).ToArray())));
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddMvc().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PRN232 LMS - Lab 2", Version = "v1" });
    options.DescribeAllParametersInCamelCase();
    options.OperationFilter<PRN232.LMS.API.Swagger.UsageDocumentation>();
    options.SchemaFilter<PRN232.LMS.API.Swagger.StudentDocumentation>();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT",
        Description = "Login at /api/v1/auth/login and paste accessToken here."
    });
    options.OperationFilter<PRN232.LMS.API.Swagger.AuthorizationDocumentation>();
});
var app = builder.Build();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages(async context => await context.HttpContext.Response.WriteAsJsonAsync(
    ApiResponseModel.Error($"HTTP {context.HttpContext.Response.StatusCode}")));
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/students", out var rest))
        context.Request.Path = "/api/v1/students" + rest;
    await next();
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
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
