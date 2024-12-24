using HopFrame.Security.Models;

namespace HopFrame.Api.Models;

public class HopFrameApiModuleConfig : HopFrameConfig {
    public bool ExposeModelEndpoints { get; set; } = true;
    public bool ExposeAuthEndpoints { get; set; } = true;
}