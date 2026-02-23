using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;

namespace HopFrame.Core.Services;

/// A service used to access configs and repositories provided by the <see cref="HopFrameConfig"/>
public interface IConfigAccessor {

    /// <summary>
    /// Searches through the config and returns the table with the specified identifier if it exists
    /// </summary>
    /// <param name="identifier">The identifier of the table</param>
    public TableConfig? GetTableByIdentifier(string identifier);
    
    /// <summary>
    /// Searches through the config and returns the table with the specified route if it exists
    /// </summary>
    /// <param name="route">The route of the table</param>
    public TableConfig? GetTableByRoute(string route);
    
    /// <summary>
    /// Searches through the config and returns the table with the specified type if it exists
    /// </summary>
    /// <param name="type">The model type for the table</param>
    public TableConfig? GetTableByType(Type type);

    /// <summary>
    /// Loads the repository for the specified table
    /// </summary>
    /// <param name="table">The table to load the repository for</param>
    public IHopFrameRepository LoadRepository(TableConfig table);

}