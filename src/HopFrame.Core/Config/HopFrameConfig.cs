using HopFrame.Core.Callbacks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Core.Config;

public class HopFrameConfig {
    public List<DbContextConfig> Contexts { get; } = new();
    public bool DisplayUserInfo { get; set; } = true;
    public string? BasePolicy { get; set; }
    public string? LoginPageRewrite { get; set; }
    public List<HopCallbackHandler> Handlers { get; } = new();
}

/// <summary>
/// A helper class for editing the <see cref="HopFrameConfig"/>
/// </summary>
public sealed class HopFrameConfigurator(HopFrameConfig config, IServiceCollection collection = null!) {
    
    /// <summary>
    /// The Internal HopFrame configuration that's modified by the helper functions
    /// </summary>
    public HopFrameConfig InnerConfig { get; } = config;

    public IServiceCollection ServiceCollection { get; } = collection;
    
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
        var context = new DbContextConfig(typeof(TDbContext), InnerConfig);
        InnerConfig.Contexts.Add(context);
        return new DbContextConfigurator<TDbContext>(context);
    }

    /// <summary>
    /// Check if a context is already registered in the HopFrame
    /// </summary>
    /// <typeparam name="TDbContext">The context that should be checked</typeparam>
    /// <returns>true if the context is already registered, false if not</returns>
    public bool HasDbContext<TDbContext>() where TDbContext : DbContext {
        return InnerConfig.Contexts.Any(context => context.ContextType == typeof(TDbContext));
    }

    /// <summary>
    /// Returns a configurator for the context if it was already defined
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns>The configurator of the context if it already was defined, null if not</returns>
    public DbContextConfigurator<TDbContext>? GetDbContext<TDbContext>() where TDbContext : DbContext {
        var config = InnerConfig.Contexts
            .SingleOrDefault(context => context.ContextType == typeof(TDbContext));
        if (config is null) return null;

        return new DbContextConfigurator<TDbContext>(config);
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
