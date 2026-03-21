using HopFrame.Core.Configuration;

namespace HopFrame.Core.Events;

/// <summary>
/// The event that is fired, when an entity is updated
/// </summary>
public interface IEntityUpdatedEventHandler : IHopFrameEventHandler {
    
    /// <inheritdoc cref="IEntityCreatedEventHandler.EntityCreated"/>
    Task EntityUpdated(object entity, TableConfig config, CancellationToken ct);
    
}

/// <inheritdoc cref="IEntityUpdatedEventHandler"/>
public interface IEntityUpdatedEventHandler<in TModel> : IEntityUpdatedEventHandler where TModel : class {
    
    Task IEntityUpdatedEventHandler.EntityUpdated(object entity, TableConfig config, CancellationToken ct) {
        if (entity is not TModel parsed)
            return Task.CompletedTask;
        
        return EntityUpdated(parsed, config, ct);
    }

    /// <inheritdoc cref="IEntityUpdatedEventHandler.EntityUpdated"/>
    Task EntityUpdated(TModel entity, TableConfig config, CancellationToken ct);
    
}
