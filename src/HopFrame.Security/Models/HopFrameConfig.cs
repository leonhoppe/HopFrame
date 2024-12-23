using HopFrame.Security.Authentication.OpenID.Implementation;

namespace HopFrame.Security.Models;

public class HopFrameConfig {
    public Type CacheProvider { get; set; } = typeof(MemoryCacheProvider);
}