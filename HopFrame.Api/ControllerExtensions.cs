using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Api;

public static class ControllerExtensions {

    public static IMvcBuilder AddController<TController>(this IMvcBuilder builder) where TController : ControllerBase {
        //TODO: Change implementation method
        return builder.AddApplicationPart(typeof(TController).Assembly);
    }
    
}