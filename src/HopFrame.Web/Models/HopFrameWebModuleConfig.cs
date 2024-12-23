using HopFrame.Security.Models;

namespace HopFrame.Web.Models;

public class HopFrameWebModuleConfig : HopFrameConfig {
    public string AdminLoginPageUri { get; set; } = "/administration/login";
}