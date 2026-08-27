using HopFrame.Core.Configuration;

namespace HopFrame.Core.Events;

/// <summary>
/// The event that is fired, when a new instance of an entity needs to be created
/// </summary>
public interface IEntityCreateCallbackHandler : IHopFrameCallback {

    Task<object> IHopFrameCallback.ExecuteCallback(object entity, TableConfig config, CancellationToken ct) {
        return CreateEntity(entity, config, ct);
    }

    /// <inheritdoc cref="IHopFrameCallback.ExecuteCallback"/>
    Task<object> CreateEntity(object entity, TableConfig config, CancellationToken ct);

}

/// <inheritdoc cref="IEntityCreateCallbackHandler"/>
public interface IEntityCreateCallbackHandler<TModel> : IEntityCreateCallbackHandler where TModel : class {

    async Task<object> IEntityCreateCallbackHandler.CreateEntity(object entity, TableConfig config, CancellationToken ct) {
        if (entity is not TModel parsed)
            return entity;

        return await CreateEntity(parsed, config, ct);
    }

    /// <inheritdoc cref="IEntityCreateCallbackHandler.CreateEntity"/>
    Task<TModel> CreateEntity(TModel entity, TableConfig config, CancellationToken ct);

}
