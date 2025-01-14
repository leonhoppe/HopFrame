using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Config;

public class HopFrameConfig {
    public List<DbContextConfig> Contexts { get; init; } = new();
    public bool DisplayUserInfo { get; set; } = true;
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
}
