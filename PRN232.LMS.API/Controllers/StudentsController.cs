using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.RequestModels;
using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.Services;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]

[ProducesResponseType(typeof(ApiResponseModel), 400), ProducesResponseType(typeof(ApiResponseModel), 404)]
public sealed class StudentsController(ILmsService service) : ControllerBase
{
    [HttpGet, ProducesResponseType(typeof(ApiResponseModel), 200)]
    public async Task<IActionResult> List([FromQuery] ListRequestModel query, [FromHeader(Name = "X-Request-Id")] string? requestId, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(requestId)) Response.Headers["X-Request-Id"] = requestId;
        return Ok(ApiResponseModel.List(await service.ListAsync("students", query.ToBusiness(), ct)));
    }

    [HttpGet("{id:int}", Name = "GetStudentById"), ProducesResponseType(typeof(ApiResponseModel), 200)]
    public async Task<IActionResult> Detail([FromRoute] int id, CancellationToken ct) => Ok(ApiResponseModel.Ok(await service.DetailAsync("students", id, ct)));

    [Authorize(Roles = "Admin")]
    [HttpPost, ProducesResponseType(typeof(ApiResponseModel), 201)]
    public async Task<IActionResult> Create([FromBody] StudentRequestModel input, CancellationToken ct)
    {
        var result = await service.CreateAsync(input.ToBusiness(), ct);
        return CreatedAtRoute("GetStudentById", new { version = "1", id = result.Values["studentId"] }, ApiResponseModel.Ok(result));
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}"), ProducesResponseType(typeof(ApiResponseModel), 200)]
    public async Task<IActionResult> Update(int id, StudentRequestModel input, CancellationToken ct) => Ok(ApiResponseModel.Ok(await service.UpdateAsync(id, input.ToBusiness(), ct)));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}"), ProducesResponseType(typeof(ApiResponseModel), 200)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(new ApiResponseModel(true, "Student deleted.", null, null));
    }
}
