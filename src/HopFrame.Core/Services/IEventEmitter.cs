using HopFrame.Core.Configuration;
using HopFrame.Core.Events;

namespace HopFrame.Core.Services;

/// <summary>
/// The service used to emit the events
/// </summary>
public interface IEventEmitter {

    /// <summary>
    /// Executes all registered Event handler in the DI in the background
    /// </summary>
    /// <param name="type">The type of the event</param>
    /// <param name="entity">The affected entity</param>
    /// <param name="config">The affected table</param>
    /// <param name="ct">indicates that the request was canceled</param>
    void PublishEvent(EventType type, object entity, TableConfig config, CancellationToken ct);

}

/// <summary>
/// The event types that can be emitted from the HopFrame
/// </summary>
public enum EventType {
    /// <inheritdoc cref="IEntityCreatedEventHandler"/>
    EntityCreated,
    
    /// <inheritdoc cref="IEntityUpdatedEventHandler"/>
    EntityUpdated,
    
    /// <inheritdoc cref="IEntityDeletedEventHandler"/>
    EntityDeleted,
}
