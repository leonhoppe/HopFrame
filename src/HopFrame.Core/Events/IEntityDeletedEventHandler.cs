using HopFrame.Core.Configuration;

namespace HopFrame.Core.Events;

/// <summary>
/// The event that is fired, when an entity is deleted
/// </summary>
public interface IEntityDeletedEventHandler : IHopFrameEventHandler {
    
    /// <inheritdoc cref="IEntityCreatedEventHandler.EntityCreated"/>
    Task EntityDeleted(object entity, TableConfig config, CancellationToken ct);
    
}

/// <inheritdoc cref="IEntityDeletedEventHandler"/>
public interface IEntityDeletedEventHandler<in TModel> : IEntityDeletedEventHandler where TModel : class {
    
    Task IEntityDeletedEventHandler.EntityDeleted(object entity, TableConfig config, CancellationToken ct) {
        if (entity is not TModel parsed)
            return Task.CompletedTask;
        
        return EntityDeleted(parsed, config, ct);
    }

    /// <inheritdoc cref="IEntityDeletedEventHandler.EntityDeleted"/>
    Task EntityDeleted(TModel entity, TableConfig config, CancellationToken ct);
    
}
