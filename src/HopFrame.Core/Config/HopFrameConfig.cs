using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Config;

public class HopFrameConfig {
    public List<DbContextConfig> Contexts { get; } = new();
    public bool DisplayUserInfo { get; set; } = true;
    public string? BasePolicy { get; set; }
    public string? LoginPageRewrite { get; set; }
}

/// <summary>
/// A helper class for editing the <see cref="HopFrameConfig"/>
/// </summary>
public class HopFrameConfigurator(HopFrameConfig config) {
    
    /// <summary>
    /// The Internal HopFrame configuration that's modified by the helper functions
    /// </summary>
    public HopFrameConfig InnerConfig { get; } = config;
    
    /// <summary>
    /// Adds all tables defined in the DbContext to the HopFrame ui and configures it using the provided configurator
    /// </summary>
    /// <param name="configurator">Used for configuring the DbContext</param>
    /// <typeparam name="TDbContext">The DbContext from which all tables should be added</typeparam>
    /// <seealso cref="DbContextConfigurator{TDbContext}"/>
    public HopFrameConfigurator AddDbContext<TDbContext>(Action<DbContextConfigurator<TDbContext>> configurator) where TDbContext : DbContext {
        var context = AddDbContext<TDbContext>();
        configurator.Invoke(context);
        return this;
    }
    
    /// <summary>
    /// Adds all tables defined in the DbContext to the HopFrame ui and configures it using the provided configurator
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext from which all tables should be added</typeparam>
    /// <returns>The configurator used for the DbContext</returns>
    /// <seealso cref="DbContextConfigurator{TDbContext}"/>
    public DbContextConfigurator<TDbContext> AddDbContext<TDbContext>() where TDbContext : DbContext {
        var context = new DbContextConfig(typeof(TDbContext));
        InnerConfig.Contexts.Add(context);
        return new DbContextConfigurator<TDbContext>(context);
    }

    /// <summary>
    /// Determines if the name of the currently logged-in user should be displayed in the top right corner of the admin ui
    /// </summary>
    public HopFrameConfigurator DisplayUserInfo(bool display) {
        InnerConfig.DisplayUserInfo = display;
        return this;
    }

    /// <summary>
    /// Sets a default policy that every user needs to have in order to access the admin ui
    /// </summary>
    public HopFrameConfigurator SetBasePolicy(string basePolicy) {
        InnerConfig.BasePolicy = basePolicy;
        return this;
    }

    /// <summary>
    /// Sets a custom login page to redirect to if the request to the admin ui was unauthorized
    /// </summary>
    public HopFrameConfigurator SetLoginPage(string url) {
        InnerConfig.LoginPageRewrite = url;
        return this;
    }
}
