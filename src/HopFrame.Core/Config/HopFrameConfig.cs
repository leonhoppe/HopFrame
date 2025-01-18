using HopFrame.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Config;

public class HopFrameConfig {
    public List<DbContextConfig> Contexts { get; } = new();
    public bool DisplayUserInfo { get; set; } = true;
    public string? BasePolicy { get; set; }
    public string? LoginPageRewrite { get; set; }
}

public class HopFrameConfigurator(HopFrameConfig config) {
    public HopFrameConfig InnerConfig { get; } = config;
    
    public HopFrameConfigurator AddDbContext<TDbContext>(Action<DbContextConfig<TDbContext>> configurator) where TDbContext : DbContext {
        var context = AddDbContext<TDbContext>();
        configurator.Invoke(context);
        return this;
    }
    
    public DbContextConfig<TDbContext> AddDbContext<TDbContext>() where TDbContext : DbContext {
        var context = new DbContextConfig(typeof(TDbContext));
        InnerConfig.Contexts.Add(context);
        return new DbContextConfig<TDbContext>(context);
    }

    public HopFrameConfigurator DisplayUserInfo(bool display) {
        InnerConfig.DisplayUserInfo = display;
        return this;
    }

    public HopFrameConfigurator SetBasePolicy(string basePolicy) {
        InnerConfig.BasePolicy = basePolicy;
        return this;
    }

    public HopFrameConfigurator SetLoginPage(string url) {
        InnerConfig.LoginPageRewrite = url;
        return this;
    }
}
