using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.RequestModels;
using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.Services;
namespace PRN232.LMS.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ApiResponseModel), 400), ProducesResponseType(typeof(ApiResponseModel), 404)]
public sealed class SemestersController(ILmsService service) : ControllerBase
{
    [HttpGet, ProducesResponseType(typeof(ApiResponseModel), 200)]
    public async Task<IActionResult> List([FromQuery] ListRequestModel query, CancellationToken ct) => Ok(ApiResponseModel.List(await service.ListAsync("semesters", query.ToBusiness(), ct)));
    [HttpGet("{id:int}"), ProducesResponseType(typeof(ApiResponseModel), 200)]
    public async Task<IActionResult> Detail(int id, CancellationToken ct) => Ok(ApiResponseModel.Ok(await service.DetailAsync("semesters", id, ct)));
}
