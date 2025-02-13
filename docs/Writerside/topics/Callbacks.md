# Callbacks

Callbacks are a way of executing actions on curtain events in the web ui.

## Registering a callback handler

You can register a callback handler using the method provided in the [](TableConfig.md):

```c#
table.AddCallbackHandler(CallbackType.DeleteEntry, (user, services) => {
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("User {user} deleted!", user.Username);
});
```

The callback handler takes the entity that's modified and a `IServiceProvider` as arguments
and can either be synchronous or asynchronous.

## Callback types

```C#
public enum CallbackType {
    CreateEntry = 0,
    UpdateEntry = 1,
    DeleteEntry = 2
}
```

- `CallbackType.CreateEntry`: The handler gets executed, when an entity is created.
- `CallbackType.UpdateEntry`: The handler gets executed, when an entity is updated.
- `CallbackType.DeleteEntry`: The handler gets executed, when an entity is deleted.
