using HopFrame.Core.Configuration;

namespace HopFrame.Core.Events;

/// <summary>
/// The event that is fired, when am enrty is about to be edited
/// </summary>
public interface IEntityUpdateCallbackHandler : IHopFrameCallback {

    Task<object> IHopFrameCallback.ExecuteCallback(object entity, TableConfig config, CancellationToken ct) {
        return UpdateEntity(entity, config, ct);
    }

    /// <inheritdoc cref="IHopFrameCallback.ExecuteCallback"/>
    Task<object> UpdateEntity(object entity, TableConfig config, CancellationToken ct);

}

/// <inheritdoc cref="IEntityUpdateCallbackHandler"/>
public interface IEntityUpdateCallbackHandler<TModel> : IEntityUpdateCallbackHandler where TModel : class {

    async Task<object> IEntityUpdateCallbackHandler.UpdateEntity(object entity, TableConfig config, CancellationToken ct) {
        if (entity is not TModel parsed)
            return entity;

        return await UpdateEntity(parsed, config, ct);
    }

    /// <inheritdoc cref="IEntityUpdateCallbackHandler.UpdateEntity"/>
    Task<TModel> UpdateEntity(TModel entity, TableConfig config, CancellationToken ct);

}
