using HopFrame.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Config;

public class HopFrameConfig {
    public List<DbContextConfig> Contexts { get; init; } = new();
    public bool DisplayUserInfo { get; set; } = true;
    public Type? AuthHandler { get; set; }
    public string? BasePolicy { get; set; }
    public string? LoginPageRewrite { get; set; }
}

public class HopFrameConfigurator(HopFrameConfig config) {
    public HopFrameConfigurator AddDbContext<TDbContext>(Action<DbContextConfig<TDbContext>> configurator) where TDbContext : DbContext {
        var context = AddDbContext<TDbContext>();
        configurator.Invoke(context);
        return this;
    }
    
    public DbContextConfig<TDbContext> AddDbContext<TDbContext>() where TDbContext : DbContext {
        var context = new DbContextConfig<TDbContext>(typeof(TDbContext));
        config.Contexts.Add(context);
        return context;
    }

    public HopFrameConfigurator DisplayUserInfo(bool display) {
        config.DisplayUserInfo = display;
        return this;
    }

    public HopFrameConfigurator SetAuthHandler<TAuthHandler>() where TAuthHandler : IHopFrameAuthHandler {
        config.AuthHandler = typeof(TAuthHandler);
        return this;
    }

    public HopFrameConfigurator SetBasePolicy(string basePolicy) {
        config.BasePolicy = basePolicy;
        return this;
    }

    public HopFrameConfigurator SetLoginPage(string url) {
        config.LoginPageRewrite = url;
        return this;
    }
}
