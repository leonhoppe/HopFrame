using HopFrame.Core.Configuration;

namespace HopFrame.Core.Events;

/// <summary>
/// The event that is fired, when an entity is created
/// </summary>
public interface IEntityCreatedEventHandler : IHopFrameEventHandler {
    
    /// <summary>
    /// The Method that is executed, when the event is fired
    /// </summary>
    /// <param name="entity">The affected entity</param>
    /// <param name="config">The configuration of the table</param>
    /// <param name="ct">Indicates if the request was canceled</param>
    Task EntityCreated(object entity, TableConfig config, CancellationToken ct);

}

/// <inheritdoc cref="IEntityCreatedEventHandler"/>
public interface IEntityCreatedEventHandler<in TModel> : IEntityCreatedEventHandler where TModel : class {
    
    Task IEntityCreatedEventHandler.EntityCreated(object entity, TableConfig config, CancellationToken ct) {
        if (entity is not TModel parsed)
            return Task.CompletedTask;
        
        return EntityCreated(parsed, config, ct);
    }

    /// <inheritdoc cref="IEntityCreatedEventHandler.EntityCreated"/>
    Task EntityCreated(TModel entity, TableConfig config, CancellationToken ct);
    
}
