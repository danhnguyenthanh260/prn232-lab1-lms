using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace PRN232.LMS.API.Routing;

// Keep the required [controller] token while publishing lowercase resource names.
public sealed class LowercaseControllerConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller) => controller.ControllerName = controller.ControllerName.ToLowerInvariant();
}
