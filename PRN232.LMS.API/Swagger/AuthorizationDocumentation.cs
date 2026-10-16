using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace PRN232.LMS.API.Swagger;
public sealed class AuthorizationDocumentation : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        if (!metadata.OfType<IAuthorizeData>().Any() || metadata.OfType<IAllowAnonymous>().Any()) return;
        operation.Security = [new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = []
        }];
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Missing, invalid or expired access token." });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "This role is not allowed." });
    }
}
