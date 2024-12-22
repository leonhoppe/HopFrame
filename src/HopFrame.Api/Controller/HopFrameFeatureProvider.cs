using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace HopFrame.Api.Controller;

public class HopFrameFeatureProvider(params Type[] controllerTypes) : ControllerFeatureProvider {
    protected override bool IsController(TypeInfo typeInfo) {
        if (typeInfo.Namespace != typeof(HopFrameFeatureProvider).Namespace)
            return base.IsController(typeInfo);

        if (controllerTypes.All(c => c.Name != typeInfo.Name))
            return false;
        
        return base.IsController(typeInfo);
    }
}