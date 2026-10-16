using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PRN232.LMS.API.RequestModels;
using PRN232.LMS.API.ResponseModels;

namespace PRN232.LMS.API.Validation;

public sealed class StudentRequestValidator : AbstractValidator<StudentRequestModel>
{
    public StudentRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100)
            .Must(x => x.All(c => c <= 127)).WithMessage("Email must contain ASCII characters only.");
        RuleFor(x => x.DateOfBirth).NotNull();
    }
}
public sealed class StudentValidationFilter(IValidator<StudentRequestModel> validator) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var input in context.ActionArguments.Values.OfType<StudentRequestModel>())
        {
            var result = await validator.ValidateAsync(input, context.HttpContext.RequestAborted);
            if (!result.IsValid)
            {
                context.Result = new BadRequestObjectResult(ApiResponseModel.Error("Invalid input.",
                    result.Errors.Select(x => $"{x.PropertyName}: {x.ErrorMessage}").ToArray()));
                return;
            }
        }
        await next();
    }
}
