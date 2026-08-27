using HopFrame.Core.Configuration;

namespace HopFrame.Core.Events;

/// <summary>
/// The base event interface for HopFrame events
/// </summary>
public interface IHopFrameEventHandler;

/// <summary>
/// The base callback interface for HopFrame callbacks
/// </summary>
public interface IHopFrameCallback {

    /// <summary>
    /// This function acts like a pipeline, modify and return the entity
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="config">The configuration of the table</param>
    /// <param name="ct">Indicates if the request was canceled</param>
    /// <returns>The modifed entity</returns>
    Task<object> ExecuteCallback(object entity, TableConfig config, CancellationToken ct);
}
