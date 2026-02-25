using HopFrame.Core.Configuration;
using HopFrame.Core.Helpers;
using HopFrame.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Core.Configurators;

/// <summary>
/// The configurator for the <see cref="HopFrameConfig"/>
/// </summary>
public class HopFrameConfigurator(HopFrameConfig config, IServiceCollection services) {
    /// The internal config that is modified
    public HopFrameConfig Config { get; } = config;

    internal IServiceCollection Services { get; } = services;

    /// <summary>
    /// Adds a new table to the configuration based on the provided repository
    /// </summary>
    /// <typeparam name="TRepository">The repository that handles the table</typeparam>
    /// <typeparam name="TModel">The type of the model</typeparam>
    /// <param name="configurator">The configurator for the table</param>
    public HopFrameConfigurator AddRepository<TRepository, TModel>(Action<TableConfigurator<TModel>>? configurator = null) where TRepository : IHopFrameRepository where TModel : class {
        var table = ConfigurationHelper.InitializeTable(Config, typeof(TRepository), typeof(TModel));
        Config.Tables.Add(table);
        Services.TryAddScoped(typeof(TRepository));
        configurator?.Invoke(new TableConfigurator<TModel>(table));
        return this;
    }

    /// <summary>
    /// Adds a new table to the configuration
    /// </summary>
    /// <param name="config">The configuration for the table</param>
    /// <param name="configurator">The configurator for the table</param>
    /// <typeparam name="TModel">The model of the table</typeparam>
    /// <exception cref="ArgumentException">Is thrown when configuration validation fails</exception>
    public HopFrameConfigurator AddTable<TModel>(TableConfig config, Action<TableConfigurator<TModel>>? configurator = null) where TModel : class {
        if (typeof(TModel) != config.TableType)
            throw new ArgumentException($"Table type for table '{config.Identifier}' does not mach requested type '{typeof(TModel).Name}'!");
        
        var errors = ConfigurationHelper.ValidateTable(Config, config).ToArray();

        if (errors.Length != 0)
            throw new ArgumentException($"Table '{config.Identifier}' has some validation errors:\n\t{string.Join("\n\t", errors)}");
        
        Config.Tables.Add(config);
        Services.TryAddScoped(config.RepositoryType);
        configurator?.Invoke(new TableConfigurator<TModel>(config));
        return this;
    }

    /// <summary>
    /// Loads the configurator for an existing table in the configuration
    /// </summary>
    /// <param name="configurator">The configurator for the table</param>
    /// <typeparam name="TModel">The model of the table</typeparam>
    /// <exception cref="ArgumentException">Is thrown when no table with the requested type was found</exception>
    public TableConfigurator<TModel> Table<TModel>(Action<TableConfigurator<TModel>>? configurator = null) where TModel : class {
        var table = Config.Tables.FirstOrDefault(t => t.TableType == typeof(TModel));

        if (table is null)
            throw new ArgumentException($"Table '{typeof(TModel).Name}' not found");

        var modeller = new TableConfigurator<TModel>(table);
        configurator?.Invoke(modeller);
        return modeller;
    }
}