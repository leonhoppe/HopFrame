using HopFrame.Core.Configuration;
using HopFrame.Core.Events;
using HopFrame.Core.Services;
using HopFrame.Core.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Tests.Core.Services.Implementation;

public class EventEmitterTests {
    private class TestCreatedHandler : IEntityCreatedEventHandler {
        public TaskCompletionSource<bool> Called = new();

        public Task EntityCreated(object entity, TableConfig config, CancellationToken ct) {
            Called.TrySetResult(true);
            return Task.CompletedTask;
        }
    }

    private class TestUpdatedHandler : IEntityUpdatedEventHandler {
        public TaskCompletionSource<bool> Called = new();

        public Task EntityUpdated(object entity, TableConfig config, CancellationToken ct) {
            Called.TrySetResult(true);
            return Task.CompletedTask;
        }
    }

    private class TestDeletedHandler : IEntityDeletedEventHandler {
        public TaskCompletionSource<bool> Called = new();

        public Task EntityDeleted(object entity, TableConfig config, CancellationToken ct) {
            Called.TrySetResult(true);
            return Task.CompletedTask;
        }
    }

    private static TableConfig DummyTable()
        => new TableConfig {
            Identifier = "Test",
            TableType = typeof(string),
            RepositoryType = typeof(object),
            Route = "test",
            DisplayName = "Test",
            OrderIndex = 0
        };

    // -------------------------------------------------------------
    // EntityCreated
    // -------------------------------------------------------------
    [Fact]
    public async Task PublishEvent_EntityCreated_InvokesHandlers() {
        var services = new ServiceCollection();
        var handler = new TestCreatedHandler();
        services.AddSingleton<IEntityCreatedEventHandler>(handler);

        var provider = services.BuildServiceProvider();
        var emitter = new EventEmitter(provider);

        emitter.PublishEvent(EventType.EntityCreated, "x", DummyTable(), CancellationToken.None);

        Assert.True(await handler.Called.Task.WaitAsync(TimeSpan.FromSeconds(1)));
    }

    // -------------------------------------------------------------
    // EntityUpdated
    // -------------------------------------------------------------
    [Fact]
    public async Task PublishEvent_EntityUpdated_InvokesHandlers() {
        var services = new ServiceCollection();
        var handler = new TestUpdatedHandler();
        services.AddSingleton<IEntityUpdatedEventHandler>(handler);

        var provider = services.BuildServiceProvider();
        var emitter = new EventEmitter(provider);

        emitter.PublishEvent(EventType.EntityUpdated, "x", DummyTable(), CancellationToken.None);

        Assert.True(await handler.Called.Task.WaitAsync(TimeSpan.FromSeconds(1)));
    }

    // -------------------------------------------------------------
    // EntityDeleted
    // -------------------------------------------------------------
    [Fact]
    public async Task PublishEvent_EntityDeleted_InvokesHandlers() {
        var services = new ServiceCollection();
        var handler = new TestDeletedHandler();
        services.AddSingleton<IEntityDeletedEventHandler>(handler);

        var provider = services.BuildServiceProvider();
        var emitter = new EventEmitter(provider);

        emitter.PublishEvent(EventType.EntityDeleted, "x", DummyTable(), CancellationToken.None);

        Assert.True(await handler.Called.Task.WaitAsync(TimeSpan.FromSeconds(1)));
    }
}