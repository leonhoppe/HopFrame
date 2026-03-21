using HopFrame.Core.Configuration;
using HopFrame.Core.Events;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Core.Services.Implementation;

internal sealed class EventEmitter(IServiceProvider services) : IEventEmitter {
    
    public void PublishEvent(EventType type, object entity, TableConfig config, CancellationToken ct) {
        switch (type) {
            case EventType.EntityCreated:
                var createHandlers = services.GetRequiredService<IEnumerable<IEntityCreatedEventHandler>>();
                Task.Run(async () => {
                    foreach (var handler in createHandlers) {
                        await handler.EntityCreated(entity, config, ct);
                    }
                }, ct);
                break;
            
            case EventType.EntityUpdated:
                var updateHandlers = services.GetRequiredService<IEnumerable<IEntityUpdatedEventHandler>>();
                Task.Run(async () => {
                    foreach (var handler in updateHandlers) {
                        await handler.EntityUpdated(entity, config, ct);
                    }
                }, ct);
                break;
            
            case EventType.EntityDeleted:
                var deleteHandlers = services.GetRequiredService<IEnumerable<IEntityDeletedEventHandler>>();
                Task.Run(async () => {
                    foreach (var handler in deleteHandlers) {
                        await handler.EntityDeleted(entity, config, ct);
                    }
                }, ct);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
    
}