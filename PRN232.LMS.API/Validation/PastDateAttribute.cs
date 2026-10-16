using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.Validation;

public sealed class PastDateAttribute : ValidationAttribute
{
    public PastDateAttribute() => ErrorMessage = "{0} must be between 1753-01-01 and today.";
    public override bool IsValid(object? value) => value is null || value is DateTime date
        && date >= new DateTime(1753, 1, 1) && date.Date <= DateTime.UtcNow.Date;
}
