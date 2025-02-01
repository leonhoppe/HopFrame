using HopFrame.Core.Config;

namespace HopFrame.Core.Events;

public static class EventTypes {

    private const string Prefix = "HopFrame.";
    private const string CreateEntryPrefix = Prefix + "Entry.Create.";
    private const string UpdateEntryPrefix = Prefix + "Entry.Update.";
    private const string DeleteEntryPrefix = Prefix + "Entry.Delete.";

    public static string CreateEntry(TableConfig config) => CreateEntryPrefix + config.PropertyName;
    public static string UpdateEntry(TableConfig config) => UpdateEntryPrefix + config.PropertyName;
    public static string DeleteEntry(TableConfig config) => DeleteEntryPrefix + config.PropertyName;

    public static string ConstructEventName(EventType type, TableConfig config) {
        return type switch {
            EventType.CreateEntry => CreateEntry(config),
            EventType.UpdateEntry => UpdateEntry(config),
            EventType.DeleteEntry => DeleteEntry(config),
            _ => Prefix
        };
    }

}

public enum EventType {
    CreateEntry = 0,
    UpdateEntry = 1,
    DeleteEntry = 2
}
