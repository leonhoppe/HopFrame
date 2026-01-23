using System.Linq.Expressions;
using HopFrame.Core.Callbacks;
using HopFrame.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Core.Config;

public class HopFrameConfig {
    public List<ITableGroupConfig> Contexts { get; } = new();
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

    /// <summary>
    /// The <see cref="ServiceCollection"/> of the application.
    /// WARNING: Only use this during application building phase
    /// </summary>
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
    /// Adds a table of the desired type and configures it to use a custom repository
    /// </summary>
    /// <param name="keyExpression">The key of the model</param>
    /// <param name="configurator">The configurator used for configuring the table page</param>
    /// <typeparam name="TRepository">The repository class that inherits from the <see cref="IHopFrameRepository{TModel,TKey}"/> (needs to be registered as a service)</typeparam>
    /// <typeparam name="TModel">The model of the table</typeparam>
    /// <typeparam name="TKey">The type of the primary key</typeparam>
    public HopFrameConfigurator AddCustomRepository<TRepository, TModel, TKey>(Expression<Func<TModel, TKey>> keyExpression, Action<TableConfigurator<TModel>> configurator) {
        var context = AddCustomRepository<TRepository, TModel, TKey>(keyExpression);
        configurator.Invoke(context);
        return this;
    }

    /// <summary>
    /// Adds a table of the desired type and configures it to use a custom repository
    /// </summary>
    /// <param name="keyExpression">The key of the model</param>
    /// <typeparam name="TRepository">The repository class that inherits from the <see cref="IHopFrameRepository{TModel,TKey}"/> (needs to be registered as a service)</typeparam>
    /// <typeparam name="TModel">The model of the table</typeparam>
    /// <typeparam name="TKey">The type of the primary key</typeparam>
    /// <returns>The configurator used for configuring the table page</returns>
    public TableConfigurator<TModel> AddCustomRepository<TRepository, TModel, TKey>(Expression<Func<TModel, TKey>> keyExpression) {
        var keyProperty = TableConfigurator<TModel>.GetPropertyInfo(keyExpression);
        var context = new RepositoryGroupConfig(typeof(TRepository), keyProperty, InnerConfig);
        context.Tables.Add(new TableConfig(context, typeof(TModel), typeof(TRepository).Name, 0));
        InnerConfig.Contexts.Add(context);
        return new TableConfigurator<TModel>(context.Tables[0]);
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
            .OfType<DbContextConfig>()
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
