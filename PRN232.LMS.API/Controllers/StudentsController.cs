using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.RequestModels;
using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.Services;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/students")]
[ProducesResponseType(typeof(ApiResponseModel), 400), ProducesResponseType(typeof(ApiResponseModel), 404)]
public sealed class StudentsController(ILmsService service) : ControllerBase
{
    [HttpGet, ProducesResponseType(typeof(ApiResponseModel), 200)]
    public async Task<IActionResult> List([FromQuery] ListRequestModel query, CancellationToken ct) => Ok(ApiResponseModel.List(await service.ListAsync("students", query.ToBusiness(), ct)));

    [HttpGet("{id:int}"), ProducesResponseType(typeof(ApiResponseModel), 200)]
    public async Task<IActionResult> Detail(int id, CancellationToken ct) => Ok(ApiResponseModel.Ok(await service.DetailAsync("students", id, ct)));

    [HttpPost, ProducesResponseType(typeof(ApiResponseModel), 201)]
    public async Task<IActionResult> Create(StudentRequestModel input, CancellationToken ct)
    {
        var result = await service.CreateAsync(input.ToBusiness(), ct);
        return CreatedAtAction(nameof(Detail), new { id = result.Values["studentId"] }, ApiResponseModel.Ok(result));
    }
    [HttpPut("{id:int}"), ProducesResponseType(typeof(ApiResponseModel), 200)]
    public async Task<IActionResult> Update(int id, StudentRequestModel input, CancellationToken ct) => Ok(ApiResponseModel.Ok(await service.UpdateAsync(id, input.ToBusiness(), ct)));

    [HttpDelete("{id:int}"), ProducesResponseType(typeof(ApiResponseModel), 200)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(new ApiResponseModel(true, "Student deleted.", null, []));
    }
}
